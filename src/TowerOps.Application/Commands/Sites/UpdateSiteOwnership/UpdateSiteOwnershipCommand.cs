using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Sites;
using TowerOps.Domain.Enums;

namespace TowerOps.Application.Commands.Sites.UpdateSiteOwnership;

public sealed record UpdateSiteOwnershipCommand : ICommand<SiteDetailDto>
{
    public string SiteCode { get; init; } = string.Empty;
    public TowerOwnershipType TowerOwnershipType { get; init; }
    public string? TowerOwnerName { get; init; }
    public string? SharingAgreementRef { get; init; }
    public string? HostContactName { get; init; }
    public string? HostContactPhone { get; init; }
}
