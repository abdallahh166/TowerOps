using TowerOps.Domain.Entities.WorkOrders;
using TowerOps.Domain.Enums;

namespace TowerOps.Domain.Services;

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
