using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Portal;

namespace TelecomPM.Application.Queries.Portal.GetPortalVisits;

public sealed record GetPortalVisitsQuery : IQuery<IReadOnlyList<PortalVisitDto>>
{
    public string SiteCode { get; init; } = string.Empty;
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 50;
}
