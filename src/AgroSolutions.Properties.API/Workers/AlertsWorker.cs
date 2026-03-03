using AgroSolutions.Properties.Application.Interfaces;
using Microsoft.Extensions.Options;

namespace AgroSolutions.AlertsProcessor.Workers
{
    public sealed class AlertsWorker : BackgroundService
    {
        private readonly IServiceProvider _provider;
        private ILogger<AlertsWorker> _logger;
        private JobOptions _job;
        private PropertiesContext _db;
        private IReadingsRepository _readingsRepo;
        private IGenerateAlertService _svc;
        private JobOptions _jobOptions;

        public AlertsWorker(IServiceProvider provider, IOptions<JobOptions> jobOptions)
        {
            _provider = provider;
            _jobOptions = jobOptions.Value;
        }

        public async Task Process(CancellationToken ct)
        {
            _logger.LogWarning("Alerts worker starting processing alerts");

            var scope = _provider.CreateScope();


            _logger = scope.ServiceProvider.GetRequiredService<ILogger<AlertsWorker>>();
            _job = scope.ServiceProvider.GetRequiredService<IOptions<JobOptions>>().Value;
            _db = scope.ServiceProvider.GetRequiredService<PropertiesContext>();
            _readingsRepo = scope.ServiceProvider.GetRequiredService<IReadingsRepository>();
            _svc = scope.ServiceProvider.GetRequiredService<IGenerateAlertService>();

            try
            {
                var fieldIds = _db.Fields.Select(x => x.Id.ToString()).ToList();

                if (fieldIds.Count == 0)
                {
                    _logger.LogWarning("No FieldIds configured. Exiting.");
                    return;
                }

                var lookbackInSeconds = Math.Max(1, _job.LookbackInSeconds);

                _logger.LogInformation("Starting job. FieldIds={Count}, Lookback={Lookback}s",
                    fieldIds.Count, lookbackInSeconds);

                var readings = await _readingsRepo.FetchReadingsAsync(fieldIds, _jobOptions.LookbackInSeconds, ct);

                await _svc.UpdateAlertsByReadings(readings);

                _logger.LogInformation("Fetched {Count} readings.", readings.Count);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                _logger.LogWarning("Job cancelled.");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Job failed.");
            }

            _logger.LogWarning("Alerts worker finishing processing alerts");
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            _logger = _provider.GetRequiredService<ILogger<AlertsWorker>>();

            _logger.LogInformation("Worker started. Interval: {Interval} seconds", _jobOptions.LookbackInSeconds);

            try
            {
                while (!ct.IsCancellationRequested)
                {
                    await Process(ct);
                    // Espera o intervalo OU cancela se o token for disparado
                    await Task.Delay(_jobOptions.LookbackInSeconds * 1000, ct);
                }

                _logger.LogInformation("Worker stopping gracefully.");
                return;
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                _logger.LogInformation("Worker cancelled.");
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Worker crashed.");
                return;
            }
        }
    }
}