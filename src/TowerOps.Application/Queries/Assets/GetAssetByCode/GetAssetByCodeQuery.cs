using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Assets;

namespace TowerOps.Application.Queries.Assets.GetAssetByCode;

public sealed record GetAssetByCodeQuery : IQuery<AssetDto>
{
    public string AssetCode { get; init; } = string.Empty;
}
