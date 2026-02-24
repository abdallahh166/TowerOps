using ClosedXML.Excel;
using FluentValidation;
using MediatR;
using TelecomPM.Application.Commands.Imports;
using TelecomPM.Application.Common;
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Application.DTOs.Sites;
using TelecomPm.Application.Services.ExcelParsers;
using TelecomPM.Domain.Entities.Sites;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Application.Commands.Imports.ImportPowerData;

public record ImportPowerDataCommand : ICommand<ImportSiteDataResult>
{
    public byte[] FileContent { get; init; } = Array.Empty<byte>();
}

public class ImportPowerDataCommandValidator : AbstractValidator<ImportPowerDataCommand>
{
    public ImportPowerDataCommandValidator()
    {
        RuleFor(x => x.FileContent)
            .NotNull()
            .Must(x => x.Length > 0)
            .WithMessage("Excel file content is required.");
    }
}

public sealed class ImportPowerDataCommandHandler : IRequestHandler<ImportPowerDataCommand, Result<ImportSiteDataResult>>
{
    private readonly ISiteRepository _siteRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISystemSettingsService? _systemSettingsService;

    public ImportPowerDataCommandHandler(ISiteRepository siteRepository, IUnitOfWork unitOfWork)
        : this(siteRepository, unitOfWork, null)
    {
    }

    public ImportPowerDataCommandHandler(
        ISiteRepository siteRepository,
        IUnitOfWork unitOfWork,
        ISystemSettingsService? systemSettingsService)
    {
        _siteRepository = siteRepository;
        _unitOfWork = unitOfWork;
        _systemSettingsService = systemSettingsService;
    }

    public async Task<Result<ImportSiteDataResult>> Handle(ImportPowerDataCommand request, CancellationToken cancellationToken)
    {
        var options = await ImportGuardrails.ResolveOptionsAsync(_systemSettingsService, cancellationToken);
        var fileValidationError = ImportGuardrails.ValidateExcelPayload(request.FileContent, options);
        if (fileValidationError is not null)
            return Result.Failure<ImportSiteDataResult>(fileValidationError);

        var result = new ImportSiteDataResult();
        var trackedSites = new Dictionary<Guid, Site>();

        using var stream = new MemoryStream(request.FileContent);
        using var workbook = new XLWorkbook(stream);
        var rowLimitFailure = ImportGuardrails.ValidateRowLimit(
            ImportGuardrails.CountNonEmptyDataRows(workbook),
            options);
        if (rowLimitFailure is not null)
            return rowLimitFailure;

        var worksheet = ImportExcelSupport.FindWorksheet(workbook, "Power Data");
        if (worksheet is null)
            return Result.Failure<ImportSiteDataResult>("Sheet 'Power Data' was not found.");

        var siteLookup = ImportExcelSupport.BuildSiteIdLookup(await _siteRepository.GetAllAsNoTrackingAsync(cancellationToken));
        var columnMap = ImportExcelSupport.BuildColumnMap(worksheet.Row(1));

        var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;
        for (var rowNumber = 2; rowNumber <= lastRow; rowNumber++)
        {
            var row = worksheet.Row(rowNumber);
            if (row.IsEmpty())
                continue;

            var siteCode = ImportExcelSupport.GetCellText(row, columnMap, "Site Code", "Short Code", "SiteCode");
            var key = ImportExcelSupport.NormalizeSiteKey(siteCode);
            if (string.IsNullOrWhiteSpace(key))
            {
                result.SkippedCount++;
                result.Errors.Add($"Row {rowNumber}: Site Code is missing.");
                continue;
            }

            if (!siteLookup.TryGetValue(key, out var siteId))
            {
                result.SkippedCount++;
                result.Errors.Add($"Row {rowNumber}: site '{key}' not found.");
                continue;
            }

            var site = await GetTrackedSiteAsync(siteId, trackedSites, cancellationToken);
            if (site is null)
            {
                result.SkippedCount++;
                result.Errors.Add($"Row {rowNumber}: site '{key}' could not be loaded for update.");
                continue;
            }

            site.SetLegacyShortCode(key);

            var siteTypeRaw = ImportExcelSupport.GetCellText(row, columnMap, "Site Type");
            var siteName = ImportExcelSupport.GetCellText(row, columnMap, "OEG Site Name", "Site Name");
            var normalizedType = ImportExcelSupport.ParseSiteTypeOrDefault(siteTypeRaw, site.SiteType);
            site.UpdateBasicInfo(
                ImportExcelSupport.IsBlankOrNa(siteName) ? site.Name : siteName,
                ImportExcelSupport.IsBlankOrNa(siteName) ? site.OMCName : siteName,
                normalizedType);

            var teName = ImportExcelSupport.GetCellText(row, columnMap, "TE Name");
            site.SetNetworkContext(ImportExcelSupport.IsBlankOrNa(teName) ? site.TelecomEgyptName : teName, site.OperationalZone);

            var acPowerRaw = ImportExcelSupport.GetCellText(row, columnMap, "AC Power", "Power Source");
            var rectifierTypeRaw = ImportExcelSupport.GetCellText(row, columnMap, "Rectifier Type");
            var batteryRaw = ImportExcelSupport.GetCellText(row, columnMap, "Battery Type");
            var batteryStringsRaw = ImportExcelSupport.GetCellText(row, columnMap, "Battery Strings Qty", "No of String");
            var routerRaw = ImportExcelSupport.GetCellText(row, columnMap, "Routers", "# of Routers");
            var modemRaw = ImportExcelSupport.GetCellText(row, columnMap, "Modem");
            var modulesRaw = ImportExcelSupport.GetCellText(row, columnMap, "No of Modules", "Rectifier No.");
            var cabinetRaw = ImportExcelSupport.GetCellText(row, columnMap, "Cabinet Type (Y/N) Y=Rectifier in Cabinet", "Cabinet Type");
            var chargingCurrentRaw = ImportExcelSupport.GetCellText(row, columnMap, "Batteries Charnging current limit", "Charging Current Limit");

            var existingPower = site.PowerSystem ?? SitePowerSystem.Create(site.Id, PowerConfiguration.ACOnly, RectifierBrand.Other, BatteryType.VRLA);
            var parsedBattery = BatteryFieldParser.Parse(batteryRaw);
            var batteryType = ParseBatteryType(batteryRaw, existingPower.BatteryType);
            var powerConfiguration = ImportExcelSupport.ParsePowerConfigurationOrDefault(acPowerRaw, existingPower.Configuration);
            var rectifierBrand = SitePowerSystem.NormalizeRectifierBrand(rectifierTypeRaw);

            var power = SitePowerSystem.Create(site.Id, powerConfiguration, rectifierBrand, batteryType);

            var modules = ImportExcelSupport.ParseInt(modulesRaw);
            if (modules.HasValue && modules.Value > 0)
                power.SetRectifierDetails(modules.Value, existingPower.RectifierControllerType);

            var strings = ImportExcelSupport.ParseInt(batteryStringsRaw);
            if (strings.HasValue && strings.Value > 0)
            {
                var voltage = parsedBattery?.Voltage ?? (existingPower.BatteryVoltage > 0 ? existingPower.BatteryVoltage : 48);
                var ampHour = parsedBattery?.AmpereHour ?? (existingPower.BatteryAmpereHour > 0 ? existingPower.BatteryAmpereHour : 170);
                power.SetBatteryDetails(strings.Value, 1, ampHour, voltage);
            }

            power.SetBatteryMetadata(parsedBattery?.Brand ?? existingPower.BatteryBrand, existingPower.BatteryHealthStatus);
            power.SetNetworkEquipmentCounts(ImportExcelSupport.ParseInt(routerRaw), ImportExcelSupport.ParseInt(modemRaw));

            var cabinetFlag = ParseCabinetFlag(cabinetRaw);
            var cabinetVendor = ParseCabinetVendor(cabinetRaw);
            var cabinetType = ParseCabinetType(cabinetRaw);
            power.SetCabinetInfo(cabinetFlag, cabinetVendor, cabinetType);
            power.SetChargingCurrentLimit(ImportExcelSupport.ParseDecimal(chargingCurrentRaw));
            power.SetRawPowerLabels(ImportExcelSupport.IsBlankOrNa(acPowerRaw) ? existingPower.PowerSourceLabel : acPowerRaw, rectifierTypeRaw);

            site.SetPowerSystem(power);

            var gpsRaw = ImportExcelSupport.GetCellText(row, columnMap, "GPS");
            var admRaw = ImportExcelSupport.GetCellText(row, columnMap, "ADM");
            var transmission = site.Transmission ?? SiteTransmission.Create(site.Id, TransmissionType.MW, "Imported");
            transmission.SetEquipment(
                BooleanTextParser.ParseNullable(gpsRaw) ?? transmission.HasGPS,
                BooleanTextParser.ParseNullable(admRaw) ?? transmission.HasADM,
                transmission.HasSDH,
                transmission.HasEBand,
                transmission.HasALURouter);

            if (site.Transmission is null)
                site.SetTransmission(transmission);

            site.SetEnclosureInfo(ImportExcelSupport.ParseEnclosureType(cabinetRaw), ImportExcelSupport.IsBlankOrNa(cabinetRaw) ? site.EnclosureTypeRaw : cabinetRaw);

            result.ImportedCount++;
        }

        if (result.ImportedCount > 0)
            await _unitOfWork.SaveChangesAsync(cancellationToken);

        var strictFailure = ImportGuardrails.EnforceSkipInvalidRows(result, options);
        if (strictFailure is not null)
            return strictFailure;

        return Result.Success(result);
    }

    private async Task<Site?> GetTrackedSiteAsync(Guid siteId, Dictionary<Guid, Site> trackedSites, CancellationToken cancellationToken)
    {
        if (trackedSites.TryGetValue(siteId, out var site))
            return site;

        site = await _siteRepository.GetByIdAsync(siteId, cancellationToken);
        if (site is not null)
            trackedSites[siteId] = site;

        return site;
    }

    private static BatteryType ParseBatteryType(string? value, BatteryType fallback)
    {
        if (ImportExcelSupport.IsBlankOrNa(value))
            return fallback;

        var normalized = value!.Trim().ToUpperInvariant();
        if (normalized.Contains("LITH")) return BatteryType.Lithium;
        if (normalized.Contains("GEL")) return BatteryType.Gel;
        if (normalized.Contains("MARATHON")) return BatteryType.Marathon;
        if (normalized.Contains("AGM")) return BatteryType.AGM;
        if (normalized.Contains("VRLA") || normalized.Contains("SBS")) return BatteryType.VRLA;
        return fallback;
    }

    private static bool? ParseCabinetFlag(string? cabinetRaw)
    {
        if (ImportExcelSupport.IsBlankOrNa(cabinetRaw))
            return null;

        var normalized = cabinetRaw!.Trim().ToUpperInvariant();
        if (normalized.StartsWith("Y")) return true;
        if (normalized.StartsWith("N")) return false;
        return null;
    }

    private static string? ParseCabinetVendor(string? cabinetRaw)
    {
        if (ImportExcelSupport.IsBlankOrNa(cabinetRaw))
            return null;

        var parts = cabinetRaw!.Split('/', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > 1 ? parts[1] : null;
    }

    private static string? ParseCabinetType(string? cabinetRaw)
    {
        if (ImportExcelSupport.IsBlankOrNa(cabinetRaw))
            return null;

        return cabinetRaw!.Trim();
    }
}
