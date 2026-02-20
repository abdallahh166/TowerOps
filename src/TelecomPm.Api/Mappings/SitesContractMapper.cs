namespace TelecomPm.Api.Mappings;

using TelecomPm.Api.Contracts.Sites;
using TelecomPM.Application.Commands.Sites.AssignEngineerToSite;
using TelecomPM.Application.Commands.Sites.CreateSite;
using TelecomPM.Application.Commands.Sites.UnassignEngineerFromSite;
using TelecomPM.Application.Commands.Sites.UpdateSite;
using TelecomPM.Application.Commands.Sites.UpdateSiteStatus;
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

    public static CreateSiteCommand ToCommand(this CreateSiteRequest request)
        => new()
        {
            SiteCode = request.SiteCode,
            Name = request.Name,
            OMCName = request.OMCName,
            OfficeId = request.OfficeId,
            Region = request.Region,
            SubRegion = request.SubRegion,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Street = request.Address.Street ?? string.Empty,
            City = request.Address.City,
            AddressRegion = request.Address.Region,
            AddressDetails = request.Address.Details ?? string.Empty,
            SiteType = request.SiteType,
            BSCName = request.BSCName,
            BSCCode = request.BSCCode
        };

    public static UpdateSiteCommand ToCommand(this UpdateSiteRequest request, Guid siteId)
        => new()
        {
            SiteId = siteId,
            Name = request.Name ?? string.Empty,
            OMCName = request.OMCName ?? string.Empty,
            SiteType = request.SiteType ?? 0,
            BSCName = request.BSCName,
            BSCCode = request.BSCCode,
            Subcontractor = request.Subcontractor,
            MaintenanceArea = request.MaintenanceArea
        };

    public static UpdateSiteStatusCommand ToCommand(this UpdateSiteStatusRequest request, Guid siteId)
        => new()
        {
            SiteId = siteId,
            Status = request.Status
        };

    public static AssignEngineerToSiteCommand ToCommand(this AssignEngineerRequest request, Guid siteId, Guid assignedBy)
        => new()
        {
            SiteId = siteId,
            EngineerId = request.EngineerId,
            AssignedBy = assignedBy
        };

    public static UnassignEngineerFromSiteCommand ToUnassignCommand(this Guid siteId)
        => new()
        {
            SiteId = siteId
        };
}
