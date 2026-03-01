namespace TowerOps.Domain.Interfaces.Repositories;

using TowerOps.Domain.Entities.WorkOrders;
using TowerOps.Domain.Enums;

public interface IWorkOrderRepository : IRepository<WorkOrder, Guid>
{
    Task<WorkOrder?> GetByWoNumberAsync(string woNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WorkOrder>> GetByUserOwnershipAsNoTrackingAsync(
        Guid userId,
        int take = 1000,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WorkOrder>> GetOpenForSlaEvaluationAsync(
        int take,
        CancellationToken cancellationToken = default);
    Task<int> CountClosedAsync(
        string? officeCode,
        SlaClass? slaClass,
        DateTime? fromDateUtc,
        DateTime? toDateUtc,
        CancellationToken cancellationToken = default);
    Task<int> CountClosedWithReworkOrReopenedHistoryAsync(
        string? officeCode,
        SlaClass? slaClass,
        DateTime? fromDateUtc,
        DateTime? toDateUtc,
        CancellationToken cancellationToken = default);
    Task<int> CountClosedWithReopenedHistoryAsync(
        string? officeCode,
        SlaClass? slaClass,
        DateTime? fromDateUtc,
        DateTime? toDateUtc,
        CancellationToken cancellationToken = default);
    Task<int> CountAtRiskAsync(
        string? officeCode,
        SlaClass? slaClass,
        DateTime? fromDateUtc,
        DateTime? toDateUtc,
        int cmAtRiskThresholdPercent,
        int pmAtRiskThresholdPercent,
        DateTime nowUtc,
        CancellationToken cancellationToken = default);
    Task<decimal> GetClosedMeanTimeToRepairHoursAsync(
        string? officeCode,
        SlaClass? slaClass,
        DateTime? fromDateUtc,
        DateTime? toDateUtc,
        CancellationToken cancellationToken = default);
}
