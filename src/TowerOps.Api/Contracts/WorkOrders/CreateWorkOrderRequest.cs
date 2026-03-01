namespace TowerOps.Api.Contracts.WorkOrders;

using TowerOps.Domain.Enums;

public sealed class CreateWorkOrderRequest
{
    public string WoNumber { get; set; } = string.Empty;
    public string SiteCode { get; set; } = string.Empty;
    public string OfficeCode { get; set; } = string.Empty;
    public WorkOrderType WorkOrderType { get; set; } = WorkOrderType.CM;
    public DateTime? ScheduledVisitDateUtc { get; set; }
    public SlaClass SlaClass { get; set; }
    public WorkOrderScope Scope { get; set; } = WorkOrderScope.ClientEquipment;
    public string IssueDescription { get; set; } = string.Empty;
}
