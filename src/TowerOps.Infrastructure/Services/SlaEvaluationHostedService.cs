using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TowerOps.Application.Common.Interfaces;

namespace TowerOps.Infrastructure.Services;

public sealed class SlaEvaluationHostedService : BackgroundService
{
    private const int DefaultIntervalSeconds = 60;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SlaEvaluationHostedService> _logger;

    public SlaEvaluationHostedService(
        IServiceScopeFactory scopeFactory,
        ILogger<SlaEvaluationHostedService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SLA evaluation hosted service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            var interval = TimeSpan.FromSeconds(DefaultIntervalSeconds);

            try
            {
                using var scope = _scopeFactory.CreateScope();
                var settings = scope.ServiceProvider.GetRequiredService<ISystemSettingsService>();
                var processor = scope.ServiceProvider.GetRequiredService<SlaEvaluationProcessor>();

                var isEnabled = await settings.GetAsync(
                    "SLA:Evaluation:Enabled",
                    true,
                    stoppingToken);

                var configuredIntervalSeconds = await settings.GetAsync(
                    "SLA:Evaluation:IntervalSeconds",
                    DefaultIntervalSeconds,
                    stoppingToken);

                interval = TimeSpan.FromSeconds(Math.Clamp(configuredIntervalSeconds, 15, 3600));

                if (isEnabled)
                {
                    var processedCount = await processor.EvaluateBatchAsync(stoppingToken);
                    _logger.LogDebug("SLA evaluation cycle completed. Processed={ProcessedCount}", processedCount);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SLA evaluation cycle failed.");
            }

            try
            {
                await Task.Delay(interval, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }

        _logger.LogInformation("SLA evaluation hosted service stopped.");
    }
}
