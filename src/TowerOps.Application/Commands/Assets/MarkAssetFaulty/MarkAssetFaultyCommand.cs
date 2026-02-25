using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Assets;

namespace TowerOps.Application.Commands.Assets.MarkAssetFaulty;

public sealed record MarkAssetFaultyCommand : ICommand<AssetDto>
{
    public string AssetCode { get; init; } = string.Empty;
    public string? Reason { get; init; }
    public string? EngineerId { get; init; }
}
