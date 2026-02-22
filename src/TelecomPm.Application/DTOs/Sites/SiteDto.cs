namespace TelecomPM.Application.DTOs.Sites;

using System;
using TelecomPM.Domain.Enums;

public record SiteDto
{
    public Guid Id { get; init; }
    public string SiteCode { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string OMCName { get; init; } = string.Empty;
    public string Region { get; init; } = string.Empty;
    public string SubRegion { get; init; } = string.Empty;
    public SiteType SiteType { get; init; }
    public SiteComplexity Complexity { get; init; }
    public SiteStatus Status { get; init; }
    public TowerOwnershipType TowerOwnershipType { get; init; }
    public ResponsibilityScope ResponsibilityScope { get; init; }
    public string? TowerOwnerName { get; init; }
    public string? HostContactName { get; init; }
    public string? HostContactPhone { get; init; }
    public int EstimatedVisitDurationMinutes { get; init; }
    public DateTime? LastVisitDate { get; init; }
    public int RequiredPhotosCount { get; init; }
}
