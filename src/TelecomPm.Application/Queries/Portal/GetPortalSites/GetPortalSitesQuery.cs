using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Portal;

namespace TelecomPM.Application.Queries.Portal.GetPortalSites;

public sealed record GetPortalSitesQuery : IQuery<IReadOnlyList<PortalSiteDto>>
{
    public string? SiteCode { get; init; }
}
