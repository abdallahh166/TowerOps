using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Portal;

namespace TowerOps.Application.Queries.Portal.GetPortalVisits;

public sealed record GetPortalVisitsQuery : IQuery<PaginatedList<PortalVisitDto>>
{
    public string SiteCode { get; init; } = string.Empty;
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 25;
    public string? SortBy { get; init; }
    public bool SortDescending { get; init; } = true;
}
