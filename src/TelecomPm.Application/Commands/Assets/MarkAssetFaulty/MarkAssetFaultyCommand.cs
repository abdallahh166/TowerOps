using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Assets;

namespace TelecomPM.Application.Commands.Assets.MarkAssetFaulty;

public sealed record MarkAssetFaultyCommand : ICommand<AssetDto>
{
    public string AssetCode { get; init; } = string.Empty;
    public string? Reason { get; init; }
    public string? EngineerId { get; init; }
}
