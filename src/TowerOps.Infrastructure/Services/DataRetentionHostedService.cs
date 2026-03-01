using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TowerOps.Application.Common.Interfaces;

namespace TowerOps.Infrastructure.Services;

public sealed class DataRetentionHostedService : BackgroundService
{
    private const int DefaultIntervalHours = 24;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DataRetentionHostedService> _logger;

    public DataRetentionHostedService(
        IServiceScopeFactory scopeFactory,
        ILogger<DataRetentionHostedService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Data retention hosted service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            var delay = TimeSpan.FromHours(DefaultIntervalHours);
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var settingsService = scope.ServiceProvider.GetRequiredService<ISystemSettingsService>();
                var processor = scope.ServiceProvider.GetRequiredService<DataRetentionProcessor>();

                var enabled = await settingsService.GetAsync(
                    "Privacy:Retention:CleanupEnabled",
                    true,
                    stoppingToken);

                var configuredIntervalHours = await settingsService.GetAsync(
                    "Privacy:Retention:CleanupIntervalHours",
                    DefaultIntervalHours,
                    stoppingToken);

                delay = TimeSpan.FromHours(Math.Clamp(configuredIntervalHours, 1, 168));

                if (enabled)
                {
                    await processor.ExecuteAsync(stoppingToken);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Data retention cycle failed.");
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

        _logger.LogInformation("Data retention hosted service stopped.");
    }
}
