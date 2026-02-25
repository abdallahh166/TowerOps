using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Portal;

namespace TowerOps.Application.Queries.Portal.GetPortalWorkOrders;

public sealed record GetPortalWorkOrdersQuery : IQuery<IReadOnlyList<PortalWorkOrderDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 50;
}
