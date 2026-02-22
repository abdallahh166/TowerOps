using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Assets;

namespace TelecomPM.Application.Queries.Assets.GetSiteAssets;

public sealed record GetSiteAssetsQuery : IQuery<IReadOnlyList<AssetDto>>
{
    public string SiteCode { get; init; } = string.Empty;
}
