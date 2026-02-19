namespace TelecomPm.Api.Mappings;

using TelecomPm.Api.Contracts.Sites;
using TelecomPM.Application.Queries.Sites.GetOfficeSites;
using TelecomPM.Application.Queries.Sites.GetSiteById;
using TelecomPM.Application.Queries.Sites.GetSitesNeedingMaintenance;

public static class SitesContractMapper
{
    public static GetSiteByIdQuery ToSiteByIdQuery(this Guid siteId)
        => new() { SiteId = siteId };

    public static GetOfficeSitesQuery ToOfficeSitesQuery(this OfficeSitesQueryParameters parameters, Guid officeId)
        => new()
        {
            OfficeId = officeId,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize,
            Complexity = parameters.Complexity,
            Status = parameters.Status
        };

    public static GetSitesNeedingMaintenanceQuery ToMaintenanceQuery(this MaintenanceSitesQueryParameters parameters)
        => new()
        {
            DaysThreshold = parameters.DaysThreshold,
            OfficeId = parameters.OfficeId
        };
}
