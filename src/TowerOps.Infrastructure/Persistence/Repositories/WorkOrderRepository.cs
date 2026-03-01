namespace TowerOps.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using TowerOps.Domain.Entities.WorkOrders;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Interfaces.Repositories;

public class WorkOrderRepository : Repository<WorkOrder, Guid>, IWorkOrderRepository
{
    public WorkOrderRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<WorkOrder?> GetByWoNumberAsync(string woNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(w => w.WoNumber == woNumber, cancellationToken);
    }

    public async Task<IReadOnlyList<WorkOrder>> GetOpenForSlaEvaluationAsync(
        int take,
        CancellationToken cancellationToken = default)
    {
        var safeTake = Math.Clamp(take, 1, 2_000);

        return await _dbSet
            .Where(wo => wo.Status != WorkOrderStatus.Closed && wo.Status != WorkOrderStatus.Cancelled)
            .OrderBy(wo => wo.CreatedAt)
            .Take(safeTake)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountClosedAsync(
        string? officeCode,
        SlaClass? slaClass,
        DateTime? fromDateUtc,
        DateTime? toDateUtc,
        CancellationToken cancellationToken = default)
    {
        return await ApplyKpiFilters(officeCode, slaClass, fromDateUtc, toDateUtc)
            .Where(wo => wo.Status == WorkOrderStatus.Closed)
            .CountAsync(cancellationToken);
    }

    public async Task<int> CountClosedWithReworkOrReopenedHistoryAsync(
        string? officeCode,
        SlaClass? slaClass,
        DateTime? fromDateUtc,
        DateTime? toDateUtc,
        CancellationToken cancellationToken = default)
    {
        var closedFilteredWorkOrders = ApplyKpiFilters(officeCode, slaClass, fromDateUtc, toDateUtc)
            .Where(wo => wo.Status == WorkOrderStatus.Closed)
            .Select(wo => wo.Id);

        return await _context.AuditLogs
            .AsNoTracking()
            .Where(a =>
                a.EntityType == "WorkOrder" &&
                (a.NewState == nameof(WorkOrderStatus.Rework) || a.NewState == "Reopened") &&
                closedFilteredWorkOrders.Contains(a.EntityId))
            .Select(a => a.EntityId)
            .Distinct()
            .CountAsync(cancellationToken);
    }

    public async Task<int> CountClosedWithReopenedHistoryAsync(
        string? officeCode,
        SlaClass? slaClass,
        DateTime? fromDateUtc,
        DateTime? toDateUtc,
        CancellationToken cancellationToken = default)
    {
        var closedFilteredWorkOrders = ApplyKpiFilters(officeCode, slaClass, fromDateUtc, toDateUtc)
            .Where(wo => wo.Status == WorkOrderStatus.Closed)
            .Select(wo => wo.Id);

        return await _context.AuditLogs
            .AsNoTracking()
            .Where(a =>
                a.EntityType == "WorkOrder" &&
                a.NewState == "Reopened" &&
                closedFilteredWorkOrders.Contains(a.EntityId))
            .Select(a => a.EntityId)
            .Distinct()
            .CountAsync(cancellationToken);
    }

    public async Task<int> CountAtRiskAsync(
        string? officeCode,
        SlaClass? slaClass,
        DateTime? fromDateUtc,
        DateTime? toDateUtc,
        int cmAtRiskThresholdPercent,
        int pmAtRiskThresholdPercent,
        DateTime nowUtc,
        CancellationToken cancellationToken = default)
    {
        var cmThresholdFraction = Math.Clamp(cmAtRiskThresholdPercent, 1, 99) / 100d;
        var pmThresholdFraction = Math.Clamp(pmAtRiskThresholdPercent, 1, 99) / 100d;

        return await ApplyKpiFilters(officeCode, slaClass, fromDateUtc, toDateUtc)
            .Where(wo =>
                wo.Status != WorkOrderStatus.Closed &&
                wo.Status != WorkOrderStatus.Cancelled &&
                nowUtc <= wo.ResponseDeadlineUtc &&
                (
                    (wo.WorkOrderType == WorkOrderType.CM &&
                     (EF.Functions.DateDiffMinute(wo.SlaStartAtUtc, nowUtc) * 1d) >=
                     (EF.Functions.DateDiffMinute(wo.SlaStartAtUtc, wo.ResponseDeadlineUtc) * cmThresholdFraction))
                    ||
                    (wo.WorkOrderType == WorkOrderType.PM &&
                     (EF.Functions.DateDiffMinute(wo.SlaStartAtUtc, nowUtc) * 1d) >=
                     (EF.Functions.DateDiffMinute(wo.SlaStartAtUtc, wo.ResponseDeadlineUtc) * pmThresholdFraction))
                ))
            .CountAsync(cancellationToken);
    }

    public async Task<decimal> GetClosedMeanTimeToRepairHoursAsync(
        string? officeCode,
        SlaClass? slaClass,
        DateTime? fromDateUtc,
        DateTime? toDateUtc,
        CancellationToken cancellationToken = default)
    {
        var meanMinutes = await ApplyKpiFilters(officeCode, slaClass, fromDateUtc, toDateUtc)
            .Where(wo => wo.Status == WorkOrderStatus.Closed && wo.UpdatedAt.HasValue)
            .Select(wo => (double?)EF.Functions.DateDiffMinute(wo.CreatedAt, wo.UpdatedAt!.Value))
            .AverageAsync(cancellationToken);

        if (!meanMinutes.HasValue || meanMinutes.Value <= 0)
            return 0m;

        return Math.Round((decimal)(meanMinutes.Value / 60d), 2);
    }

    private IQueryable<WorkOrder> ApplyKpiFilters(
        string? officeCode,
        SlaClass? slaClass,
        DateTime? fromDateUtc,
        DateTime? toDateUtc)
    {
        var query = _dbSet.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(officeCode))
            query = query.Where(wo => wo.OfficeCode == officeCode);

        if (slaClass.HasValue)
            query = query.Where(wo => wo.SlaClass == slaClass.Value);

        if (fromDateUtc.HasValue)
            query = query.Where(wo => wo.CreatedAt >= fromDateUtc.Value);

        if (toDateUtc.HasValue)
            query = query.Where(wo => wo.CreatedAt <= toDateUtc.Value);

        return query;
    }
}
