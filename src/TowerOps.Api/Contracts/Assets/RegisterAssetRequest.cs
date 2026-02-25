using TowerOps.Domain.Enums;

namespace TowerOps.Api.Contracts.Assets;

public sealed class RegisterAssetRequest
{
    public string SiteCode { get; init; } = string.Empty;
    public AssetType Type { get; init; }
    public string? Brand { get; init; }
    public string? Model { get; init; }
    public string? SerialNumber { get; init; }
    public DateTime InstalledAtUtc { get; init; } = DateTime.UtcNow;
    public DateTime? WarrantyExpiresAtUtc { get; init; }
}
