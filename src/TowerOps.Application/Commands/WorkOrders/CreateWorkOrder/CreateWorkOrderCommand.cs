using System;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.WorkOrders;
using TowerOps.Domain.Enums;

namespace TowerOps.Application.Commands.WorkOrders.CreateWorkOrder;

public record CreateWorkOrderCommand : ICommand<WorkOrderDto>
{
    public string WoNumber { get; init; } = string.Empty;
    public string SiteCode { get; init; } = string.Empty;
    public string OfficeCode { get; init; } = string.Empty;
    public WorkOrderType WorkOrderType { get; init; } = WorkOrderType.CM;
    public DateTime? ScheduledVisitDateUtc { get; init; }
    public SlaClass SlaClass { get; init; }
    public WorkOrderScope Scope { get; init; } = WorkOrderScope.ClientEquipment;
    public string IssueDescription { get; init; } = string.Empty;
}
