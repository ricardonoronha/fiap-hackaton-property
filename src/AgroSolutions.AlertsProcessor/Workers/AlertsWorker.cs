using AgroSolutions.Properties.Application.Interfaces;
using InfluxDB.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AgroSolutions.AlertsProcessor.Workers
{
    public sealed class AlertsWorker
    {
        private readonly IServiceProvider _provider;
        private ILogger<AlertsWorker> _logger;
        private JobOptions _job;
        private PropertiesContext _db;
        private IReadingsRepository _readingsRepo;
        private IGenerateAlertService _svc;

        public AlertsWorker(IServiceProvider provider)
        {
            _provider = provider;
        }

        public async Task<int> RunAsync(CancellationToken ct)
        {
            var interval = TimeSpan.FromMinutes(
                Math.Max(1, _provider
                    .GetRequiredService<IOptions<JobOptions>>()
                    .Value.LookbackMinutes));

            _logger = _provider.GetRequiredService<ILogger<AlertsWorker>>();

            _logger.LogInformation("Worker started. Interval: {Interval} minutes", interval.TotalMinutes);

            try
            {
                while (!ct.IsCancellationRequested)
                {
                    await Process(ct);
                    // Espera o intervalo OU cancela se o token for disparado
                    await Task.Delay(interval, ct);
                }

                _logger.LogInformation("Worker stopping gracefully.");
                return 0;
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                _logger.LogInformation("Worker cancelled.");
                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Worker crashed.");
                return 1;
            }
        }

        public async Task Process(CancellationToken ct)
        {
            _logger = _provider.GetRequiredService<ILogger<AlertsWorker>>();
            _job = _provider.GetRequiredService<IOptions<JobOptions>>().Value;
            _db = _provider.GetRequiredService<PropertiesContext>();
            _readingsRepo = _provider.GetRequiredService<IReadingsRepository>();
            _svc = _provider.GetRequiredService<IGenerateAlertService>();

            try
            {
                var fieldIds = _db.Fields.Select(x => x.Id.ToString()).ToList();

                if (fieldIds.Count == 0)
                {
                    _logger.LogWarning("No FieldIds configured. Exiting.");
                    return;
                }

                var lookbackMinutes = Math.Max(1, _job.LookbackMinutes);
                var lookback = TimeSpan.FromMinutes(lookbackMinutes);

                _logger.LogInformation("Starting job. FieldIds={Count}, Lookback={Lookback}m",
                    fieldIds.Count, lookbackMinutes);

                var readings = await _readingsRepo.FetchReadingsAsync(fieldIds, lookback, ct);

                await _svc.UpdateAlertsByReadings(readings);

                _logger.LogInformation("Fetched {Count} readings.", readings.Count);

                return;
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                _logger.LogWarning("Job cancelled.");
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Job failed.");
                return;
            }
        }
    }
}