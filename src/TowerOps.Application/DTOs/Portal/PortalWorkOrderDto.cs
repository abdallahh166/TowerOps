using TowerOps.Domain.Enums;

namespace TowerOps.Application.DTOs.Portal;

public sealed record PortalWorkOrderDto
{
    public Guid WorkOrderId { get; init; }
    public string SiteCode { get; init; } = string.Empty;
    public WorkOrderStatus Status { get; init; }
    public SlaClass Priority { get; init; }
    public DateTime SlaDeadline { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? CompletedAt { get; init; }
}
