using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Assets;

namespace TelecomPM.Application.Queries.Assets.GetAssetHistory;

public sealed record GetAssetHistoryQuery : IQuery<AssetDto>
{
    public string AssetCode { get; init; } = string.Empty;
}
