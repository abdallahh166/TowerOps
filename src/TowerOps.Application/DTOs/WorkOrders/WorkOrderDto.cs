using System;
using TowerOps.Domain.Enums;

namespace TowerOps.Application.DTOs.WorkOrders;

public class WorkOrderDto
{
    public Guid Id { get; set; }
    public string WoNumber { get; set; } = string.Empty;
    public string SiteCode { get; set; } = string.Empty;
    public string OfficeCode { get; set; } = string.Empty;
    public SlaClass SlaClass { get; set; }
    public WorkOrderScope Scope { get; set; }
    public WorkOrderStatus Status { get; set; }
    public string IssueDescription { get; set; } = string.Empty;
    public Guid? AssignedEngineerId { get; set; }
    public string? AssignedEngineerName { get; set; }
    public DateTime? AssignedAtUtc { get; set; }
    public DateTime ResponseDeadlineUtc { get; set; }
    public DateTime ResolutionDeadlineUtc { get; set; }
}
