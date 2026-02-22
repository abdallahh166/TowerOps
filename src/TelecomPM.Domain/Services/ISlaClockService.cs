using TelecomPM.Domain.Entities.WorkOrders;
using TelecomPM.Domain.Enums;

namespace TelecomPM.Domain.Services;

public interface ISlaClockService
{
    Task<bool> IsBreachedAsync(
        DateTime createdAtUtc,
        int responseMinutes,
        SlaClass slaClass,
        CancellationToken cancellationToken = default);

    Task<DateTime> CalculateDeadlineAsync(
        DateTime createdAtUtc,
        SlaClass slaClass,
        CancellationToken cancellationToken = default);

    Task<SlaStatus> EvaluateStatusAsync(WorkOrder workOrder, CancellationToken cancellationToken = default);
}
