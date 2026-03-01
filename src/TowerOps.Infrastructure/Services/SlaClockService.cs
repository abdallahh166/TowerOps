using Microsoft.Extensions.Caching.Memory;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Domain.Entities.WorkOrders;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Services;

namespace TowerOps.Infrastructure.Services;

public class SlaClockService : ISlaClockService
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);
    private readonly ISystemSettingsService _settingsService;
    private readonly IMemoryCache _memoryCache;

    public SlaClockService(
        ISystemSettingsService settingsService,
        IMemoryCache memoryCache)
    {
        _settingsService = settingsService;
        _memoryCache = memoryCache;
    }

    public async Task<bool> IsBreachedAsync(
        DateTime createdAtUtc,
        int responseMinutes,
        SlaClass slaClass,
        CancellationToken cancellationToken = default)
    {
        if (slaClass == SlaClass.P4)
            return false;

        var respondedAtUtc = createdAtUtc.AddMinutes(responseMinutes);
        var deadlineUtc = await CalculateDeadlineAsync(createdAtUtc, slaClass, cancellationToken);
        return respondedAtUtc > deadlineUtc;
    }

    public async Task<DateTime> CalculateDeadlineAsync(
        DateTime createdAtUtc,
        SlaClass slaClass,
        CancellationToken cancellationToken = default)
    {
        return slaClass switch
        {
            SlaClass.P1 => createdAtUtc.AddMinutes(await GetResponseMinutesAsync(slaClass, 60, cancellationToken)),
            SlaClass.P2 => createdAtUtc.AddMinutes(await GetResponseMinutesAsync(slaClass, 240, cancellationToken)),
            SlaClass.P3 => createdAtUtc.AddMinutes(await GetResponseMinutesAsync(slaClass, 1440, cancellationToken)),
            SlaClass.P4 => DateTime.MaxValue,
            _ => createdAtUtc.AddMinutes(await GetResponseMinutesAsync(slaClass, 1440, cancellationToken))
        };
    }

    public async Task<SlaStatus> EvaluateStatusAsync(WorkOrder workOrder, CancellationToken cancellationToken = default)
    {
        if (workOrder.SlaClass == SlaClass.P4)
        {
            workOrder.ApplySlaStatus(SlaStatus.OnTime, DateTime.UtcNow);
            return SlaStatus.OnTime;
        }

        var nowUtc = DateTime.UtcNow;
        var deadlineUtc = workOrder.ResponseDeadlineUtc;
        var atRiskThresholdPercent = await GetAtRiskThresholdPercentAsync(workOrder.WorkOrderType, cancellationToken);
        var totalWindowMinutes = Math.Max(1d, (workOrder.ResponseDeadlineUtc - workOrder.SlaStartAtUtc).TotalMinutes);
        var atRiskStartUtc = workOrder.SlaStartAtUtc.AddMinutes(totalWindowMinutes * (atRiskThresholdPercent / 100d));
        var isAtRiskByPercent = nowUtc >= atRiskStartUtc;

        var status = nowUtc > deadlineUtc
            ? SlaStatus.Breached
            : isAtRiskByPercent
                ? SlaStatus.AtRisk
                : SlaStatus.OnTime;

        workOrder.ApplySlaStatus(status, nowUtc);
        return status;
    }

    private async Task<int> GetResponseMinutesAsync(
        SlaClass slaClass,
        int defaultValue,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"sla-response:{slaClass}";

        var cachedValue = await _memoryCache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CacheDuration;
            var settingKey = slaClass switch
            {
                SlaClass.P1 => "SLA:P1:ResponseMinutes",
                SlaClass.P2 => "SLA:P2:ResponseMinutes",
                SlaClass.P3 => "SLA:P3:ResponseMinutes",
                SlaClass.P4 => "SLA:P4:ResponseMinutes",
                _ => string.Empty
            };

            if (string.IsNullOrWhiteSpace(settingKey))
            {
                return defaultValue;
            }

            return await _settingsService.GetAsync(settingKey, defaultValue, cancellationToken);
        });

        return cachedValue;
    }

    private async Task<int> GetAtRiskThresholdPercentAsync(
        WorkOrderType workOrderType,
        CancellationToken cancellationToken)
    {
        var typePrefix = workOrderType == WorkOrderType.PM ? "PM" : "CM";
        var cacheKey = $"sla-at-risk-threshold-percent:{typePrefix}";
        const int defaultThresholdPercent = 80;

        var cachedValue = await _memoryCache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CacheDuration;
            var typeSpecific = await _settingsService.GetAsync(
                $"SLA:{typePrefix}:AtRiskThresholdPercent",
                defaultThresholdPercent,
                cancellationToken);

            if (typeSpecific > 0)
            {
                return typeSpecific;
            }

            return await _settingsService.GetAsync(
                "SLA:AtRiskThresholdPercent",
                defaultThresholdPercent,
                cancellationToken);
        });

        var thresholdPercent = cachedValue;
        return Math.Clamp(thresholdPercent, 1, 99);
    }
}
