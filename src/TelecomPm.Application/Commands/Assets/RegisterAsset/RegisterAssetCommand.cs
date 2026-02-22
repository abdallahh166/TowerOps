using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Assets;
using TelecomPM.Domain.Enums;

namespace TelecomPM.Application.Commands.Assets.RegisterAsset;

public sealed record RegisterAssetCommand : ICommand<AssetDto>
{
    public string SiteCode { get; init; } = string.Empty;
    public AssetType Type { get; init; }
    public string? Brand { get; init; }
    public string? Model { get; init; }
    public string? SerialNumber { get; init; }
    public DateTime InstalledAtUtc { get; init; } = DateTime.UtcNow;
    public DateTime? WarrantyExpiresAtUtc { get; init; }
}
