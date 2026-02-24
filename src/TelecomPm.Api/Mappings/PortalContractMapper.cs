using TelecomPM.Application.Queries.Portal.GetPortalDashboard;
using TelecomPM.Application.Queries.Portal.GetPortalVisitEvidence;
using TelecomPM.Application.Queries.Portal.GetPortalSites;
using TelecomPM.Application.Queries.Portal.GetPortalSlaReport;
using TelecomPM.Application.Queries.Portal.GetPortalVisits;
using TelecomPM.Application.Queries.Portal.GetPortalWorkOrders;

namespace TelecomPm.Api.Mappings;

public static class PortalContractMapper
{
    public static GetPortalDashboardQuery ToPortalDashboardQuery(this object _)
        => new();

    public static GetPortalSitesQuery ToPortalSitesQuery(int pageNumber, int pageSize)
        => new() { PageNumber = pageNumber, PageSize = pageSize };

    public static GetPortalSitesQuery ToPortalSiteByCodeQuery(this string siteCode)
        => new() { SiteCode = siteCode, PageNumber = 1, PageSize = 1 };

    public static GetPortalWorkOrdersQuery ToPortalWorkOrdersQuery(this object _, int pageNumber, int pageSize)
        => new() { PageNumber = pageNumber, PageSize = pageSize };

    public static GetPortalSlaReportQuery ToPortalSlaReportQuery(this object _)
        => new();

    public static GetPortalVisitsQuery ToPortalVisitsQuery(this string siteCode, int pageNumber, int pageSize)
        => new() { SiteCode = siteCode, PageNumber = pageNumber, PageSize = pageSize };

    public static GetPortalVisitEvidenceQuery ToPortalVisitEvidenceQuery(this Guid visitId)
        => new() { VisitId = visitId };
}
