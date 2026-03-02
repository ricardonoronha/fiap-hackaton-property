public sealed class InfluxOptions
{
    public string Url { get; init; } = default!;
    public string Token { get; init; } = default!;
    public string Org { get; init; } = default!;
    public string Bucket { get; init; } = default!;
    public string Measurement { get; init; } = default!;
}

public sealed class JobOptions
{
    public int LookbackInSeconds { get; init; } = 7200;
}