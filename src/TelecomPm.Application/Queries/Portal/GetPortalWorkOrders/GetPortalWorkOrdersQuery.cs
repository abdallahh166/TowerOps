using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Portal;

namespace TelecomPM.Application.Queries.Portal.GetPortalWorkOrders;

public sealed record GetPortalWorkOrdersQuery : IQuery<IReadOnlyList<PortalWorkOrderDto>>;
