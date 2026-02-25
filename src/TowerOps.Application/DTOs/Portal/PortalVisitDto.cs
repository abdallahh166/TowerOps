using TowerOps.Domain.Enums;

namespace TowerOps.Application.DTOs.Portal;

public sealed record PortalVisitDto
{
    public Guid VisitId { get; init; }
    public string VisitNumber { get; init; } = string.Empty;
    public VisitStatus Status { get; init; }
    public VisitType Type { get; init; }
    public DateTime ScheduledDate { get; init; }
    public string EngineerDisplayName { get; init; } = "Field Engineer";
}
