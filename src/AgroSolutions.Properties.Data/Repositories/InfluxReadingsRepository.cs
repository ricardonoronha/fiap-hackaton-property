using InfluxDB.Client;
using Microsoft.Extensions.Options;
using System;
using System.Globalization;
using System.Text;

public sealed class InfluxReadingsRepository : IReadingsRepository
{
    private const string valueField = "value";
    private const string fieldIdColumn = "fieldId";
    private const string sensorTypeColumn = "sensorType";

    private readonly InfluxOptions _opt;

    public InfluxReadingsRepository(IOptions<InfluxOptions> opt)
        => _opt = opt.Value;

    public async Task<List<SensorReadingDto>> FetchReadingsAsync(
        IReadOnlyCollection<string> fieldIds,
        int LookbackInSeconds,
        CancellationToken ct)
    {
        if (fieldIds is null || fieldIds.Count == 0)
            return new List<SensorReadingDto>();

        var from = DateTime.Now.AddSeconds(LookbackInSeconds * -1);

        // Monta array Flux: ["id1","id2",...]
        var fluxIds = BuildFluxStringArray(fieldIds);

        // Ajuste esses 3 conforme seu schema:
        string measurement = _opt.Measurement;
        

        var flux = $"""
        import "array"

        fieldIds = {fluxIds}

        from(bucket: "{_opt.Bucket}")
          |> range(start: {from:o})
          |> filter(fn: (r) => r._measurement == "{measurement}")
          |> filter(fn: (r) => r._field == "{valueField}")
          |> filter(fn: (r) => contains(value: r.{fieldIdColumn}, set: fieldIds))
          |> keep(columns: ["_time", "_value", "{fieldIdColumn}", "{sensorTypeColumn}"])
        """;

        using var client = new InfluxDBClient(_opt.Url, _opt.Token);
        var tables = await client.GetQueryApi().QueryAsync(flux, _opt.Org, ct);

        var result = new List<SensorReadingDto>(capacity: 1024);

        foreach (var table in tables)
        {
            foreach (var record in table.Records)
            {
                var time = record.GetTimeInDateTime() ?? default;
                var valueObj = record.GetValue();

                // Influx pode voltar long/double/etc
                var value = valueObj switch
                {
                    int i => i,
                    long l => checked((int)l),
                    double d => checked((int)d),
                    float f => checked((int)f),
                    string s => int.Parse(s, CultureInfo.InvariantCulture),
                    _ => Convert.ToInt32(valueObj, CultureInfo.InvariantCulture)
                };

                record.Values.TryGetValue(fieldIdColumn, out var fidObj);
                record.Values.TryGetValue(sensorTypeColumn, out var stObj);

                var fieldId = fidObj?.ToString() ?? "";
                var sensorType = stObj?.ToString() ?? "";

                result.Add(new SensorReadingDto(fieldId, sensorType, value, DateTime.SpecifyKind(time, DateTimeKind.Utc)));
            }
        }

        return result;
    }

    private static string BuildFluxStringArray(IEnumerable<string> values)
    {
        // Flux string precisa de aspas e escape de aspas/backslash
        // ["a","b","c"]
        var sb = new StringBuilder();
        sb.Append('[');

        var first = true;
        foreach (var v in values)
        {
            if (string.IsNullOrWhiteSpace(v)) continue;

            if (!first) sb.Append(',');
            first = false;

            var escaped = v.Replace("\\", "\\\\").Replace("\"", "\\\"");
            sb.Append('"').Append(escaped).Append('"');
        }

        sb.Append(']');
        return sb.ToString();
    }
}