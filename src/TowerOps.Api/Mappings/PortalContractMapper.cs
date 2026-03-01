using TowerOps.Application.Queries.Portal.GetPortalDashboard;
using TowerOps.Application.Queries.Portal.GetPortalVisitEvidence;
using TowerOps.Application.Queries.Portal.GetPortalSites;
using TowerOps.Application.Queries.Portal.GetPortalSlaReport;
using TowerOps.Application.Queries.Portal.GetPortalVisits;
using TowerOps.Application.Queries.Portal.GetPortalWorkOrders;

namespace TowerOps.Api.Mappings;

public static class PortalContractMapper
{
    public static GetPortalDashboardQuery ToPortalDashboardQuery(this object _)
        => new();

    public static GetPortalSitesQuery ToPortalSitesQuery(int page, int pageSize, string? sortBy, bool sortDescending)
        => new() { Page = page, PageSize = pageSize, SortBy = sortBy, SortDescending = sortDescending };

    public static GetPortalSitesQuery ToPortalSiteByCodeQuery(this string siteCode)
        => new() { SiteCode = siteCode, Page = 1, PageSize = 1 };

    public static GetPortalWorkOrdersQuery ToPortalWorkOrdersQuery(this object _, int page, int pageSize, string? sortBy, bool sortDescending)
        => new() { Page = page, PageSize = pageSize, SortBy = sortBy, SortDescending = sortDescending };

    public static GetPortalSlaReportQuery ToPortalSlaReportQuery(this object _)
        => new();

    public static GetPortalVisitsQuery ToPortalVisitsQuery(this string siteCode, int page, int pageSize, string? sortBy, bool sortDescending)
        => new() { SiteCode = siteCode, Page = page, PageSize = pageSize, SortBy = sortBy, SortDescending = sortDescending };

    public static GetPortalVisitEvidenceQuery ToPortalVisitEvidenceQuery(this Guid visitId)
        => new() { VisitId = visitId };
}
