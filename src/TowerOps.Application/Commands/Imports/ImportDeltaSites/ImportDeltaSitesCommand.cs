using ClosedXML.Excel;
using FluentValidation;
using MediatR;
using TowerOps.Application.Commands.Imports;
using TowerOps.Application.Common;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Application.DTOs.Sites;
using TowerOps.Application.Services.ExcelParsers;
using TowerOps.Domain.Entities.Sites;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Interfaces.Repositories;

namespace TowerOps.Application.Commands.Imports.ImportDeltaSites;

public record ImportDeltaSitesCommand : ICommand<ImportSiteDataResult>
{
    public byte[] FileContent { get; init; } = Array.Empty<byte>();
}

public class ImportDeltaSitesCommandValidator : AbstractValidator<ImportDeltaSitesCommand>
{
    public ImportDeltaSitesCommandValidator()
    {
        RuleFor(x => x.FileContent)
            .NotNull()
            .Must(x => x.Length > 0)
            .WithMessage("Excel file content is required.");
    }
}

public sealed class ImportDeltaSitesCommandHandler : IRequestHandler<ImportDeltaSitesCommand, Result<ImportSiteDataResult>>
{
    private readonly ISiteRepository _siteRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISystemSettingsService? _systemSettingsService;

    public ImportDeltaSitesCommandHandler(ISiteRepository siteRepository, IUnitOfWork unitOfWork)
        : this(siteRepository, unitOfWork, null)
    {
    }

    public ImportDeltaSitesCommandHandler(
        ISiteRepository siteRepository,
        IUnitOfWork unitOfWork,
        ISystemSettingsService? systemSettingsService)
    {
        _siteRepository = siteRepository;
        _unitOfWork = unitOfWork;
        _systemSettingsService = systemSettingsService;
    }

    public async Task<Result<ImportSiteDataResult>> Handle(ImportDeltaSitesCommand request, CancellationToken cancellationToken)
    {
        var options = await ImportGuardrails.ResolveOptionsAsync(_systemSettingsService, cancellationToken);
        var fileValidationError = ImportGuardrails.ValidateExcelPayload(request.FileContent, options);
        if (fileValidationError is not null)
            return Result.Failure<ImportSiteDataResult>(fileValidationError);

        var result = new ImportSiteDataResult();

        using var stream = new MemoryStream(request.FileContent);
        using var workbook = new XLWorkbook(stream);
        var rowLimitFailure = ImportGuardrails.ValidateRowLimit(
            ImportGuardrails.CountNonEmptyDataRows(workbook),
            options);
        if (rowLimitFailure is not null)
            return rowLimitFailure;

        var trackedSites = new Dictionary<Guid, Site>();
        var siteLookup = ImportExcelSupport.BuildSiteIdLookup(await _siteRepository.GetAllAsNoTrackingAsync(cancellationToken));

        var sheet1 = ImportExcelSupport.FindWorksheet(workbook, "Sheet1");
        if (sheet1 is not null)
            await ImportSheet1Async(sheet1, siteLookup, trackedSites, result, cancellationToken);

        var sheet2 = ImportExcelSupport.FindWorksheet(workbook, "Sheet2");
        if (sheet2 is not null)
            await ImportSheet2Async(sheet2, siteLookup, trackedSites, result, cancellationToken);

        if (sheet1 is null && sheet2 is null)
            return Result.Failure<ImportSiteDataResult>("Sheets 'Sheet1' and 'Sheet2' were not found.");

        if (result.ImportedCount > 0)
            await _unitOfWork.SaveChangesAsync(cancellationToken);

        var strictFailure = ImportGuardrails.EnforceSkipInvalidRows(result, options);
        if (strictFailure is not null)
            return strictFailure;

        return Result.Success(result);
    }

    private async Task ImportSheet1Async(
        IXLWorksheet worksheet,
        Dictionary<string, Guid> siteLookup,
        Dictionary<Guid, Site> trackedSites,
        ImportSiteDataResult result,
        CancellationToken cancellationToken)
    {
        var columnMap = ImportExcelSupport.BuildColumnMap(worksheet.Row(1));
        var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;

        for (var rowNumber = 2; rowNumber <= lastRow; rowNumber++)
        {
            var row = worksheet.Row(rowNumber);
            if (row.IsEmpty())
                continue;

            var shortCode = ImportExcelSupport.GetCellText(row, columnMap, "Short Code");
            var key = ImportExcelSupport.NormalizeSiteKey(shortCode);

            if (string.IsNullOrWhiteSpace(key) || !siteLookup.TryGetValue(key, out var siteId))
            {
                result.SkippedCount++;
                result.Errors.Add($"Sheet1 row {rowNumber}: site '{key}' not found.");
                continue;
            }

            var site = await GetTrackedSiteAsync(siteId, trackedSites, cancellationToken);
            if (site is null)
            {
                result.SkippedCount++;
                result.Errors.Add($"Sheet1 row {rowNumber}: site '{key}' could not be loaded for update.");
                continue;
            }

            site.SetLegacyShortCode(key);

            var operationalZone = ImportExcelSupport.GetCellText(row, columnMap, "OZ");
            site.SetNetworkContext(site.TelecomEgyptName, ImportExcelSupport.IsBlankOrNa(operationalZone) ? site.OperationalZone : operationalZone);

            var nodalRaw = ImportExcelSupport.GetCellText(row, columnMap, "Nodal Deg.", "Nodal Deg", "Nodal Degree");
            var transmission = site.Transmission ?? SiteTransmission.Create(site.Id, TransmissionType.MW, "Imported");
            transmission.SetNodalDegree(nodalRaw);
            if (site.Transmission is null)
                site.SetTransmission(transmission);

            result.ImportedCount++;
        }
    }

    private async Task ImportSheet2Async(
        IXLWorksheet worksheet,
        Dictionary<string, Guid> siteLookup,
        Dictionary<Guid, Site> trackedSites,
        ImportSiteDataResult result,
        CancellationToken cancellationToken)
    {
        var columnMap = ImportExcelSupport.BuildColumnMap(worksheet.Row(1));
        var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;

        for (var rowNumber = 2; rowNumber <= lastRow; rowNumber++)
        {
            var row = worksheet.Row(rowNumber);
            if (row.IsEmpty())
                continue;

            var shortCode = ImportExcelSupport.GetCellText(row, columnMap, "Short Code");
            var key = ImportExcelSupport.NormalizeSiteKey(shortCode);

            if (string.IsNullOrWhiteSpace(key) || !siteLookup.TryGetValue(key, out var siteId))
            {
                result.SkippedCount++;
                result.Errors.Add($"Sheet2 row {rowNumber}: site '{key}' not found.");
                continue;
            }

            var site = await GetTrackedSiteAsync(siteId, trackedSites, cancellationToken);
            if (site is null)
            {
                result.SkippedCount++;
                result.Errors.Add($"Sheet2 row {rowNumber}: site '{key}' could not be loaded for update.");
                continue;
            }

            site.SetLegacyShortCode(key);

            var nodalRaw = ImportExcelSupport.GetCellText(row, columnMap, "Nodal Degree");
            var rectifierRaw = ImportExcelSupport.GetCellText(row, columnMap, "Rectifier Brand");
            var batteryRaw = ImportExcelSupport.GetCellText(row, columnMap, "Battery Type/Volt/AH");
            var batteryStatus = ImportExcelSupport.GetCellText(row, columnMap, "Batteries Status");
            var batteryStringsRaw = ImportExcelSupport.GetCellText(row, columnMap, "No of String");

            var transmission = site.Transmission ?? SiteTransmission.Create(site.Id, TransmissionType.MW, "Imported");
            transmission.SetNodalDegree(nodalRaw);
            if (site.Transmission is null)
                site.SetTransmission(transmission);

            var existingPower = site.PowerSystem ?? SitePowerSystem.Create(site.Id, PowerConfiguration.ACOnly, RectifierBrand.Other, BatteryType.VRLA);
            var parsedBattery = BatteryFieldParser.Parse(batteryRaw);

            var batteryType = ParseBatteryType(batteryRaw, existingPower.BatteryType);
            var power = SitePowerSystem.Create(
                site.Id,
                existingPower.Configuration,
                SitePowerSystem.NormalizeRectifierBrand(rectifierRaw),
                batteryType);

            power.SetRawPowerLabels(existingPower.PowerSourceLabel, rectifierRaw);
            power.SetBatteryMetadata(parsedBattery?.Brand ?? existingPower.BatteryBrand, ImportExcelSupport.IsBlankOrNa(batteryStatus) ? existingPower.BatteryHealthStatus : batteryStatus);
            power.SetNetworkEquipmentCounts(existingPower.RouterCount, existingPower.ModemCount);

            var stringCount = ImportExcelSupport.ParseInt(batteryStringsRaw);
            if (stringCount.HasValue && stringCount.Value > 0)
            {
                power.SetBatteryDetails(
                    stringCount.Value,
                    1,
                    parsedBattery?.AmpereHour ?? (existingPower.BatteryAmpereHour > 0 ? existingPower.BatteryAmpereHour : 170),
                    parsedBattery?.Voltage ?? (existingPower.BatteryVoltage > 0 ? existingPower.BatteryVoltage : 48));
            }

            site.SetPowerSystem(power);
            result.ImportedCount++;
        }
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
}
