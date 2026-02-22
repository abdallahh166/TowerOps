using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Portal;

namespace TelecomPM.Application.Queries.Portal.GetPortalSites;

public sealed record GetPortalSitesQuery : IQuery<IReadOnlyList<PortalSiteDto>>
{
    public string? SiteCode { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 50;
}
