using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Assets;

namespace TowerOps.Application.Commands.Assets.ReplaceAsset;

public sealed record ReplaceAssetCommand : ICommand<AssetDto>
{
    public string AssetCode { get; init; } = string.Empty;
    public Guid NewAssetId { get; init; }
}
