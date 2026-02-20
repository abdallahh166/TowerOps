using TelecomPM.Domain.Entities.WorkOrders;
using TelecomPM.Domain.Enums;

namespace TelecomPM.Domain.Specifications.WorkOrderSpecifications;

public sealed class OperationsDashboardWorkOrdersSpecification : BaseSpecification<WorkOrder>
{
    public OperationsDashboardWorkOrdersSpecification(
        string? officeCode,
        SlaClass? slaClass,
        DateTime? fromDateUtc,
        DateTime? toDateUtc,
        bool onlyOpen = false,
        bool onlyBreached = false,
        bool onlyAtRisk = false,
        DateTime? nowUtc = null)
        : base(wo =>
            (string.IsNullOrWhiteSpace(officeCode) || wo.OfficeCode.Equals(officeCode, StringComparison.OrdinalIgnoreCase)) &&
            (!slaClass.HasValue || wo.SlaClass == slaClass.Value) &&
            (!fromDateUtc.HasValue || wo.CreatedAt >= fromDateUtc.Value) &&
            (!toDateUtc.HasValue || wo.CreatedAt <= toDateUtc.Value) &&
            (!onlyOpen || (wo.Status != WorkOrderStatus.Closed && wo.Status != WorkOrderStatus.Cancelled)) &&
            (!onlyBreached || ((wo.Status != WorkOrderStatus.Closed && wo.Status != WorkOrderStatus.Cancelled) && nowUtc.HasValue && wo.ResolutionDeadlineUtc < nowUtc.Value)) &&
            (!onlyAtRisk || ((wo.Status != WorkOrderStatus.Closed && wo.Status != WorkOrderStatus.Cancelled) &&
                             nowUtc.HasValue &&
                             wo.ResolutionDeadlineUtc >= nowUtc.Value &&
                             wo.ResolutionDeadlineUtc <= nowUtc.Value.AddHours(2))))
    {
    }
}
