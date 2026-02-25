namespace TowerOps.Application.Queries.Sites.GetSiteLocation;

using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Sites;

public sealed record GetSiteLocationQuery : IQuery<SiteLocationDto>
{
    public string SiteCode { get; init; } = string.Empty;
}
