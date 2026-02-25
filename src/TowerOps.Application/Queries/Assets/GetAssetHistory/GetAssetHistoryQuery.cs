using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Assets;

namespace TowerOps.Application.Queries.Assets.GetAssetHistory;

public sealed record GetAssetHistoryQuery : IQuery<AssetDto>
{
    public string AssetCode { get; init; } = string.Empty;
}
