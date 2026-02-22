using TelecomPM.Application.Queries.Portal.GetPortalDashboard;
using TelecomPM.Application.Queries.Portal.GetPortalSites;
using TelecomPM.Application.Queries.Portal.GetPortalSlaReport;
using TelecomPM.Application.Queries.Portal.GetPortalVisits;
using TelecomPM.Application.Queries.Portal.GetPortalWorkOrders;

namespace TelecomPm.Api.Mappings;

public static class PortalContractMapper
{
    public static GetPortalDashboardQuery ToPortalDashboardQuery(this object _)
        => new();

    public static GetPortalSitesQuery ToPortalSitesQuery()
        => new();

    public static GetPortalSitesQuery ToPortalSiteByCodeQuery(this string siteCode)
        => new() { SiteCode = siteCode };

    public static GetPortalWorkOrdersQuery ToPortalWorkOrdersQuery(this object _)
        => new();

    public static GetPortalSlaReportQuery ToPortalSlaReportQuery(this object _)
        => new();

    public static GetPortalVisitsQuery ToPortalVisitsQuery(this string siteCode)
        => new() { SiteCode = siteCode };
}
