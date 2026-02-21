using Microsoft.Extensions.Caching.Memory;
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Domain.Entities.WorkOrders;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Services;

namespace TelecomPM.Infrastructure.Services;

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

    public bool IsBreached(DateTime createdAtUtc, int responseMinutes, SlaClass slaClass)
    {
        if (slaClass == SlaClass.P4)
            return false;

        var respondedAtUtc = createdAtUtc.AddMinutes(responseMinutes);
        return respondedAtUtc > CalculateDeadline(createdAtUtc, slaClass);
    }

    public DateTime CalculateDeadline(DateTime createdAtUtc, SlaClass slaClass)
    {
        return slaClass switch
        {
            SlaClass.P1 => createdAtUtc.AddMinutes(GetResponseMinutes(slaClass, 60)),
            SlaClass.P2 => createdAtUtc.AddMinutes(GetResponseMinutes(slaClass, 240)),
            SlaClass.P3 => createdAtUtc.AddMinutes(GetResponseMinutes(slaClass, 1440)),
            SlaClass.P4 => DateTime.MaxValue,
            _ => createdAtUtc.AddMinutes(GetResponseMinutes(slaClass, 1440))
        };
    }

    public SlaStatus EvaluateStatus(WorkOrder workOrder)
    {
        if (workOrder.SlaClass == SlaClass.P4)
        {
            workOrder.ApplySlaStatus(SlaStatus.OnTime, DateTime.UtcNow);
            return SlaStatus.OnTime;
        }

        var nowUtc = DateTime.UtcNow;
        var deadlineUtc = CalculateDeadline(workOrder.CreatedAt, workOrder.SlaClass);

        var status = nowUtc > deadlineUtc
            ? SlaStatus.Breached
            : nowUtc >= deadlineUtc.AddMinutes(-30)
                ? SlaStatus.AtRisk
                : SlaStatus.OnTime;

        workOrder.ApplySlaStatus(status, nowUtc);
        return status;
    }

    private int GetResponseMinutes(SlaClass slaClass, int defaultValue)
    {
        var cacheKey = $"sla-response:{slaClass}";

        return _memoryCache.GetOrCreate(cacheKey, entry =>
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

            return _settingsService
                .GetAsync(settingKey, defaultValue)
                .GetAwaiter()
                .GetResult();
        });
    }
}
