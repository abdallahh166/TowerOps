using TelecomPM.Domain.Enums;

namespace TelecomPm.Api.Contracts.Sites;

public sealed class UpdateSiteOwnershipRequest
{
    public TowerOwnershipType TowerOwnershipType { get; init; }
    public string? TowerOwnerName { get; init; }
    public string? SharingAgreementRef { get; init; }
    public string? HostContactName { get; init; }
    public string? HostContactPhone { get; init; }
}
