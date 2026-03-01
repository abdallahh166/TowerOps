using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TowerOps.Application.Common.Interfaces;

namespace TowerOps.Infrastructure.Services;

public sealed class UploadScanHostedService : BackgroundService
{
    private const int DefaultIntervalSeconds = 60;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<UploadScanHostedService> _logger;

    public UploadScanHostedService(
        IServiceScopeFactory scopeFactory,
        ILogger<UploadScanHostedService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Upload scan hosted service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            var delay = TimeSpan.FromSeconds(DefaultIntervalSeconds);

            try
            {
                using var scope = _scopeFactory.CreateScope();
                var settings = scope.ServiceProvider.GetRequiredService<ISystemSettingsService>();
                var processor = scope.ServiceProvider.GetRequiredService<UploadScanProcessor>();

                var isEnabled = await settings.GetAsync(
                    "UploadSecurity:Scan:Enabled",
                    true,
                    stoppingToken);

                var intervalSeconds = await settings.GetAsync(
                    "UploadSecurity:Scan:IntervalSeconds",
                    DefaultIntervalSeconds,
                    stoppingToken);

                delay = TimeSpan.FromSeconds(Math.Clamp(intervalSeconds, 10, 3600));

                if (isEnabled)
                {
                    var processed = await processor.EvaluateBatchAsync(stoppingToken);
                    _logger.LogDebug("Upload scan cycle completed. Processed={ProcessedCount}", processed);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Upload scan cycle failed.");
            }

            try
            {
                await Task.Delay(delay, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }

        _logger.LogInformation("Upload scan hosted service stopped.");
    }
}
