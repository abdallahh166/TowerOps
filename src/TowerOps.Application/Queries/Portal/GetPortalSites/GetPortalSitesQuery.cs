using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Portal;

namespace TowerOps.Application.Queries.Portal.GetPortalSites;

public sealed record GetPortalSitesQuery : IQuery<PaginatedList<PortalSiteDto>>
{
    public string? SiteCode { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 25;
    public string? SortBy { get; init; }
    public bool SortDescending { get; init; } = true;
}
