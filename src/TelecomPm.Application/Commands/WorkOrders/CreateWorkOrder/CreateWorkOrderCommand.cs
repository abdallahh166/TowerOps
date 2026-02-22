using System;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.WorkOrders;
using TelecomPM.Domain.Enums;

namespace TelecomPM.Application.Commands.WorkOrders.CreateWorkOrder;

public record CreateWorkOrderCommand : ICommand<WorkOrderDto>
{
    public string WoNumber { get; init; } = string.Empty;
    public string SiteCode { get; init; } = string.Empty;
    public string OfficeCode { get; init; } = string.Empty;
    public SlaClass SlaClass { get; init; }
    public WorkOrderScope Scope { get; init; } = WorkOrderScope.ClientEquipment;
    public string IssueDescription { get; init; } = string.Empty;
}
