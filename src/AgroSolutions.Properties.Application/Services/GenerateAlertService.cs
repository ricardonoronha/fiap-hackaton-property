using AgroSolutions.Properties.Application.Interfaces;
using AgroSolutions.Properties.Domain.Entities;
using AgroSolutions.Properties.Domain.Interfaces;

namespace AgroSolutions.Properties.Application.Services
{
    public class GenerateAlertService : IGenerateAlertService
    {

        private readonly IFieldRepository _fieldRepo;
        public GenerateAlertService(IFieldRepository fieldRepository)
        {
            _fieldRepo = fieldRepository;
        }

        public async Task UpdateAlertsByReadings(List<SensorReadingDto> readings)
        {
            if (readings is null || readings.Count == 0)
                return;

            var fields = await _fieldRepo.GetAllAsync();
            var nowUtc = DateTime.UtcNow;

            // Index: FieldId -> SensorType -> lista de readings (janela inteira)
            var readingsByField = readings
                .Where(r => !string.IsNullOrWhiteSpace(r.FieldId) && !string.IsNullOrWhiteSpace(r.SensorType))
                .GroupBy(r => r.FieldId)
                .ToDictionary(
                    g => g.Key,
                    g => g.GroupBy(x => x.SensorType)
                          .ToDictionary(
                              gg => gg.Key,
                              gg => gg.ToList()
                          )
                );

            foreach (var field in fields)
            {
                if (field.Culture is null)
                    continue;

                var fieldKey = field.Id.ToString();

                if (!readingsByField.TryGetValue(fieldKey, out var bySensor))
                    continue;

                // Ajuste os nomes para bater com o que vem do Influx
                bySensor.TryGetValue("SoilMoisture", out var moistReadings);

                moistReadings ??= new List<SensorReadingDto>();

                var hasMoist = moistReadings.Count > 0;
                
                var moistMin = hasMoist ? moistReadings.Min(x => x.Value) : (int?)null;
                var moistMax = hasMoist ? moistReadings.Max(x => x.Value) : (int?)null;

                
                var droughtTriggered = moistMin.HasValue && moistMin.Value < field.Culture.MinMoist;
                
                var activeAlerts = field.Alerts.Where(x => x.Active).ToList();

                if (!droughtTriggered)
                {
                    foreach (var a in activeAlerts.Where(a => a.Type == AlertType.Drought))
                    {
                        a.Active = false;
                        a.EndDate = nowUtc;
                    }
                }

                if (droughtTriggered)
                {
                    var hasActive = field.Alerts.Any(a => a.Active && a.Type == AlertType.Drought);

                    if (!hasActive)
                    {
                        field.Alerts.Add(new Alert
                        {
                            Id = Guid.NewGuid(),
                            FieldId = field.Id,
                            Field = field, // opcional (se EF já trackeia)
                            SensorType = "Moist",
                            Type = AlertType.Drought,
                            StartDate = nowUtc,
                            EndDate = null,
                            Active = true
                        });
                    }
                }
              
                await _fieldRepo.UpdateAsync(field.Id, field);
            }

        }
    }
}
