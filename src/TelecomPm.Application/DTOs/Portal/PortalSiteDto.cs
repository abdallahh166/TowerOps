using TelecomPM.Domain.Enums;

namespace TelecomPM.Application.DTOs.Portal;

public sealed record PortalSiteDto
{
    public string SiteCode { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public SiteStatus Status { get; init; }
    public string Region { get; init; } = string.Empty;
    public DateTime? LastVisitDate { get; init; }
    public VisitType? LastVisitType { get; init; }
    public int OpenWorkOrdersCount { get; init; }
    public int BreachedSlaCount { get; init; }
}
