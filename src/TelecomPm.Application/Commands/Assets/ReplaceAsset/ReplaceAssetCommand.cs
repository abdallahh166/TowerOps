using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Assets;

namespace TelecomPM.Application.Commands.Assets.ReplaceAsset;

public sealed record ReplaceAssetCommand : ICommand<AssetDto>
{
    public string AssetCode { get; init; } = string.Empty;
    public Guid NewAssetId { get; init; }
}
