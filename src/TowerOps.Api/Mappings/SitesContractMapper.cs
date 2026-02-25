namespace TowerOps.Api.Mappings;

using TowerOps.Api.Contracts.Sites;
using TowerOps.Application.Commands.Sites.AssignEngineerToSite;
using TowerOps.Application.Commands.Imports.ImportBatteryDischargeTest;
using TowerOps.Application.Commands.Imports.ImportDeltaSites;
using TowerOps.Application.Commands.Imports.ImportPowerData;
using TowerOps.Application.Commands.Imports.ImportRFStatus;
using TowerOps.Application.Commands.Imports.ImportSiteAssets;
using TowerOps.Application.Commands.Imports.ImportSiteRadioData;
using TowerOps.Application.Commands.Imports.ImportSiteSharingData;
using TowerOps.Application.Commands.Imports.ImportSiteTxData;
using TowerOps.Application.Commands.Sites.CreateSite;
using TowerOps.Application.Commands.Sites.ImportSiteData;
using TowerOps.Application.Commands.Sites.UnassignEngineerFromSite;
using TowerOps.Application.Commands.Sites.UpdateSite;
using TowerOps.Application.Commands.Sites.UpdateSiteOwnership;
using TowerOps.Application.Commands.Sites.UpdateSiteStatus;
using TowerOps.Application.Queries.Sites.GetOfficeSites;
using TowerOps.Application.Queries.Sites.GetSiteById;
using TowerOps.Application.Queries.Sites.GetSiteLocation;
using TowerOps.Application.Queries.Sites.GetSitesNeedingMaintenance;

public static class SitesContractMapper
{
    public static GetSiteByIdQuery ToSiteByIdQuery(this Guid siteId)
        => new() { SiteId = siteId };

    public static GetSiteLocationQuery ToSiteLocationQuery(this string siteCode)
        => new() { SiteCode = siteCode };

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

    public static UpdateSiteOwnershipCommand ToCommand(this UpdateSiteOwnershipRequest request, string siteCode)
        => new()
        {
            SiteCode = siteCode,
            TowerOwnershipType = request.TowerOwnershipType,
            TowerOwnerName = request.TowerOwnerName,
            SharingAgreementRef = request.SharingAgreementRef,
            HostContactName = request.HostContactName,
            HostContactPhone = request.HostContactPhone
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

    public static ImportSiteDataCommand ToCommand(this byte[] fileContent)
        => new()
        {
            FileContent = fileContent
        };

    public static ImportSiteAssetsCommand ToSiteAssetsImportCommand(this byte[] fileContent)
        => new() { FileContent = fileContent };

    public static ImportPowerDataCommand ToPowerDataImportCommand(this byte[] fileContent)
        => new() { FileContent = fileContent };

    public static ImportSiteRadioDataCommand ToSiteRadioDataImportCommand(this byte[] fileContent)
        => new() { FileContent = fileContent };

    public static ImportSiteTxDataCommand ToSiteTxDataImportCommand(this byte[] fileContent)
        => new() { FileContent = fileContent };

    public static ImportSiteSharingDataCommand ToSiteSharingDataImportCommand(this byte[] fileContent)
        => new() { FileContent = fileContent };

    public static ImportRFStatusCommand ToRfStatusImportCommand(this byte[] fileContent)
        => new() { FileContent = fileContent };

    public static ImportBatteryDischargeTestCommand ToBatteryDischargeTestImportCommand(this byte[] fileContent)
        => new() { FileContent = fileContent };

    public static ImportDeltaSitesCommand ToDeltaSitesImportCommand(this byte[] fileContent)
        => new() { FileContent = fileContent };
}
