using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Portal;

namespace TowerOps.Application.Queries.Portal.GetPortalWorkOrders;

public sealed record GetPortalWorkOrdersQuery : IQuery<PaginatedList<PortalWorkOrderDto>>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 25;
    public string? SortBy { get; init; }
    public bool SortDescending { get; init; } = true;
}
