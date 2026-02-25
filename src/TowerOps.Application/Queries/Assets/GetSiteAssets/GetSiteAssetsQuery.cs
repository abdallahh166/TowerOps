using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Assets;

namespace TowerOps.Application.Queries.Assets.GetSiteAssets;

public sealed record GetSiteAssetsQuery : IQuery<IReadOnlyList<AssetDto>>
{
    public string SiteCode { get; init; } = string.Empty;
}
