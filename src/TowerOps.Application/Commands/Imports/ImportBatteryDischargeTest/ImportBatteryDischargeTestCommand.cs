using ClosedXML.Excel;
using FluentValidation;
using MediatR;
using TowerOps.Application.Commands.Imports;
using TowerOps.Application.Common;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Application.DTOs.Sites;
using TowerOps.Domain.Entities.BatteryDischargeTests;
using TowerOps.Domain.Entities.Sites;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Interfaces.Repositories;

namespace TowerOps.Application.Commands.Imports.ImportBatteryDischargeTest;

public record ImportBatteryDischargeTestCommand : ICommand<ImportSiteDataResult>
{
    public byte[] FileContent { get; init; } = Array.Empty<byte>();
}

public class ImportBatteryDischargeTestCommandValidator : AbstractValidator<ImportBatteryDischargeTestCommand>
{
    public ImportBatteryDischargeTestCommandValidator()
    {
        RuleFor(x => x.FileContent)
            .NotNull()
            .Must(x => x.Length > 0)
            .WithMessage("Excel file content is required.");
    }
}

public sealed class ImportBatteryDischargeTestCommandHandler : IRequestHandler<ImportBatteryDischargeTestCommand, Result<ImportSiteDataResult>>
{
    private readonly ISiteRepository _siteRepository;
    private readonly IBatteryDischargeTestRepository _batteryDischargeTestRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISystemSettingsService? _systemSettingsService;

    public ImportBatteryDischargeTestCommandHandler(
        ISiteRepository siteRepository,
        IBatteryDischargeTestRepository batteryDischargeTestRepository,
        IUnitOfWork unitOfWork)
        : this(siteRepository, batteryDischargeTestRepository, unitOfWork, null)
    {
    }

    public ImportBatteryDischargeTestCommandHandler(
        ISiteRepository siteRepository,
        IBatteryDischargeTestRepository batteryDischargeTestRepository,
        IUnitOfWork unitOfWork,
        ISystemSettingsService? systemSettingsService)
    {
        _siteRepository = siteRepository;
        _batteryDischargeTestRepository = batteryDischargeTestRepository;
        _unitOfWork = unitOfWork;
        _systemSettingsService = systemSettingsService;
    }

    public async Task<Result<ImportSiteDataResult>> Handle(ImportBatteryDischargeTestCommand request, CancellationToken cancellationToken)
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

        var worksheet = ImportExcelSupport.FindWorksheet(workbook, "Summary", "Summary ");
        if (worksheet is null)
            return Result.Failure<ImportSiteDataResult>("Sheet 'Summary' was not found.");

        var siteLookup = ImportExcelSupport.BuildSiteLookup(await _siteRepository.GetAllAsNoTrackingAsync(cancellationToken));
        var columnMap = ImportExcelSupport.BuildColumnMap(worksheet.Row(1));

        var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;
        for (var rowNumber = 2; rowNumber <= lastRow; rowNumber++)
        {
            var row = worksheet.Row(rowNumber);
            if (row.IsEmpty())
                continue;

            var shortCode = ImportExcelSupport.GetCellText(row, columnMap, "Short Code", "Site Code");
            var key = ImportExcelSupport.NormalizeSiteKey(shortCode);

            if (string.IsNullOrWhiteSpace(key) || !siteLookup.TryGetValue(key, out var site))
            {
                result.SkippedCount++;
                result.Errors.Add($"Row {rowNumber}: site '{key}' not found.");
                continue;
            }

            var testDateRaw = ImportExcelSupport.GetCellRawValue(row, columnMap, "Test Date", "Date");
            var testDate = ImportExcelSupport.ParseDateUtc(testDateRaw);
            if (!testDate.HasValue)
            {
                result.SkippedCount++;
                result.Errors.Add($"Row {rowNumber}: test date is invalid.");
                continue;
            }

            var bdt = BatteryDischargeTest.Create(site.Id, site.SiteCode.Value, testDate.Value);
            bdt.UpdateDetails(
                relatedVisitType: ParseVisitType(ImportExcelSupport.GetCellText(row, columnMap, "Type", "Related Visit Type")),
                engineerName: ImportExcelSupport.GetCellText(row, columnMap, "Eng. Name", "Engineer", "Engineer Name"),
                subcontractorOffice: ImportExcelSupport.GetCellText(row, columnMap, "Office", "Sub-office", "Subcontractor Office"),
                network: ImportExcelSupport.GetCellText(row, columnMap, "Network"),
                siteCategory: ImportExcelSupport.GetCellText(row, columnMap, "Site Category (Shelter/OD/Grill)", "Site Category"),
                powerSource: ImportExcelSupport.GetCellText(row, columnMap, "Power Source"),
                nodalDegree: ParseNodalDegree(ImportExcelSupport.GetCellText(row, columnMap, "Nodal Degree")),
                startVoltage: ImportExcelSupport.ParseDecimal(ImportExcelSupport.GetCellText(row, columnMap, "Start Volt")),
                startAmperage: ImportExcelSupport.ParseDecimal(ImportExcelSupport.GetCellText(row, columnMap, "Start Amp")),
                endVoltage: ImportExcelSupport.ParseDecimal(ImportExcelSupport.GetCellText(row, columnMap, "End Volt")),
                endAmperage: ImportExcelSupport.ParseDecimal(ImportExcelSupport.GetCellText(row, columnMap, "End Amp")),
                plvdLlvdValue: ImportExcelSupport.ParseDecimal(ImportExcelSupport.GetCellText(row, columnMap, "PLVD Value (LLVD For Huawei) adjusted after finishing the test", "PLVD Value")),
                dischargeTimeMinutes: ImportExcelSupport.ParseInt(ImportExcelSupport.GetCellText(row, columnMap, "Discharge time( Mins)", "Discharge time")),
                reasonForStop: ImportExcelSupport.GetCellText(row, columnMap, "Reason for Test stop"),
                reasonForRepeatedBdt: ImportExcelSupport.GetCellText(row, columnMap, "Reason for Repeated BDT"),
                capRequestNumber: ImportExcelSupport.GetCellText(row, columnMap, "Cap request #"),
                notes: ImportExcelSupport.GetCellText(row, columnMap, "Comment"),
                week: ImportExcelSupport.GetCellText(row, columnMap, "Week"));

            var chargingCurrentLimit = ImportExcelSupport.ParseDecimal(
                ImportExcelSupport.GetCellText(row, columnMap, "Batteries Charnging current limit", "Charging Current Limit"));
            if (chargingCurrentLimit.HasValue)
            {
                var trackedSite = await GetTrackedSiteAsync(site.Id, trackedSites, cancellationToken);
                if (trackedSite is not null)
                {
                    var powerSystem = trackedSite.PowerSystem ??
                                      SitePowerSystem.Create(trackedSite.Id, PowerConfiguration.ACOnly, RectifierBrand.Other, BatteryType.VRLA);

                    powerSystem.SetChargingCurrentLimit(chargingCurrentLimit);
                    if (trackedSite.PowerSystem is null)
                        trackedSite.SetPowerSystem(powerSystem);
                }
            }

            await _batteryDischargeTestRepository.AddAsync(bdt, cancellationToken);
            result.ImportedCount++;
        }

        if (result.ImportedCount > 0)
            await _unitOfWork.SaveChangesAsync(cancellationToken);

        var strictFailure = ImportGuardrails.EnforceSkipInvalidRows(result, options);
        if (strictFailure is not null)
            return strictFailure;

        return Result.Success(result);
    }

    private static VisitType? ParseVisitType(string? value)
    {
        if (ImportExcelSupport.IsBlankOrNa(value))
            return null;

        var normalized = value!.Trim().ToUpperInvariant();
        if (normalized.Contains("BM")) return VisitType.BM;
        if (normalized.Contains("CM")) return VisitType.CM;
        if (normalized.Contains("AUDIT")) return VisitType.Audit;
        return null;
    }

    private static int? ParseNodalDegree(string? value)
    {
        if (ImportExcelSupport.IsBlankOrNa(value))
            return null;

        var normalized = value!.Replace(" ", string.Empty, StringComparison.Ordinal);
        var plusIndex = normalized.IndexOf('+');

        if (plusIndex > 0)
            return ImportExcelSupport.ParseInt(normalized[..plusIndex]);

        return ImportExcelSupport.ParseInt(normalized);
    }

    private async Task<Site?> GetTrackedSiteAsync(
        Guid siteId,
        Dictionary<Guid, Site> trackedSites,
        CancellationToken cancellationToken)
    {
        if (trackedSites.TryGetValue(siteId, out var site))
            return site;

        site = await _siteRepository.GetByIdAsync(siteId, cancellationToken);
        if (site is not null)
            trackedSites[siteId] = site;

        return site;
    }
}
