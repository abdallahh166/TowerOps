using ClosedXML.Excel;
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Application.Commands.Imports;
using MediatR;
using TelecomPM.Application.Commands.Sites.CreateSite;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Sites;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Interfaces.Repositories;
using TelecomPM.Domain.ValueObjects;

namespace TelecomPM.Application.Commands.Sites.ImportSiteData;

public class ImportSiteDataCommandHandler : IRequestHandler<ImportSiteDataCommand, Result<ImportSiteDataResult>>
{
    private readonly IOfficeRepository _officeRepository;
    private readonly ISender _sender;
    private readonly ISystemSettingsService? _systemSettingsService;

    public ImportSiteDataCommandHandler(IOfficeRepository officeRepository, ISender sender)
        : this(officeRepository, sender, null)
    {
    }

    public ImportSiteDataCommandHandler(
        IOfficeRepository officeRepository,
        ISender sender,
        ISystemSettingsService? systemSettingsService)
    {
        _officeRepository = officeRepository;
        _sender = sender;
        _systemSettingsService = systemSettingsService;
    }

    public async Task<Result<ImportSiteDataResult>> Handle(ImportSiteDataCommand request, CancellationToken cancellationToken)
    {
        var errors = new List<string>();
        var imported = 0;
        var skipped = 0;

        var officeCache = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);

        try
        {
            var options = await ImportGuardrails.ResolveOptionsAsync(_systemSettingsService, cancellationToken);
            var fileValidationError = ImportGuardrails.ValidateExcelPayload(request.FileContent, options);
            if (fileValidationError is not null)
                return Result.Failure<ImportSiteDataResult>(fileValidationError);

            using var stream = new MemoryStream(request.FileContent);
            using var workbook = new XLWorkbook(stream);
            var rowLimitFailure = ImportGuardrails.ValidateRowLimit(
                ImportGuardrails.CountNonEmptyDataRows(workbook),
                options);
            if (rowLimitFailure is not null)
                return rowLimitFailure;

            var worksheet = workbook.Worksheets.First();

            var headerRow = worksheet.Row(1);
            var columnMap = BuildColumnMap(headerRow);

            var requiredColumns = new[]
            {
                "SiteCode", "SiteName", "OfficeCode", "Region",
                "SubRegion", "SiteType", "Latitude", "Longitude"
            };

            var missing = requiredColumns.Where(c => !columnMap.ContainsKey(c)).ToList();
            if (missing.Count > 0)
            {
                return Result.Failure<ImportSiteDataResult>($"Missing required columns: {string.Join(", ", missing)}");
            }

            var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;
            for (var rowIndex = 2; rowIndex <= lastRow; rowIndex++)
            {
                var row = worksheet.Row(rowIndex);
                if (row.IsEmpty())
                    continue;

                var siteCode = GetCell(row, columnMap, "SiteCode");
                var siteName = GetCell(row, columnMap, "SiteName");
                var officeCode = GetCell(row, columnMap, "OfficeCode");
                var region = GetCell(row, columnMap, "Region");
                var subRegion = GetCell(row, columnMap, "SubRegion");
                var siteTypeRaw = GetCell(row, columnMap, "SiteType");
                var latitudeRaw = GetCell(row, columnMap, "Latitude");
                var longitudeRaw = GetCell(row, columnMap, "Longitude");

                if (string.IsNullOrWhiteSpace(siteCode) ||
                    string.IsNullOrWhiteSpace(siteName) ||
                    string.IsNullOrWhiteSpace(officeCode) ||
                    string.IsNullOrWhiteSpace(region) ||
                    string.IsNullOrWhiteSpace(subRegion) ||
                    string.IsNullOrWhiteSpace(siteTypeRaw) ||
                    string.IsNullOrWhiteSpace(latitudeRaw) ||
                    string.IsNullOrWhiteSpace(longitudeRaw))
                {
                    skipped++;
                    errors.Add($"Row {rowIndex}: one or more required fields are missing.");
                    continue;
                }

                try
                {
                    SiteCode.Create(siteCode);
                }
                catch (Exception ex)
                {
                    skipped++;
                    errors.Add($"Row {rowIndex}: invalid SiteCode '{siteCode}'. {ex.Message}");
                    continue;
                }

                if (!TryParseSiteType(siteTypeRaw, out var siteType))
                {
                    skipped++;
                    errors.Add($"Row {rowIndex}: invalid SiteType '{siteTypeRaw}'.");
                    continue;
                }

                if (!double.TryParse(latitudeRaw, out var latitude) ||
                    !double.TryParse(longitudeRaw, out var longitude))
                {
                    skipped++;
                    errors.Add($"Row {rowIndex}: invalid latitude/longitude.");
                    continue;
                }

                if (!officeCache.TryGetValue(officeCode, out var officeId))
                {
                    var office = await _officeRepository.GetByCodeAsNoTrackingAsync(officeCode, cancellationToken);
                    if (office is null)
                    {
                        skipped++;
                        errors.Add($"Row {rowIndex}: office code '{officeCode}' not found.");
                        continue;
                    }

                    officeId = office.Id;
                    officeCache[officeCode] = officeId;
                }

                var createResult = await _sender.Send(new CreateSiteCommand
                {
                    SiteCode = siteCode,
                    Name = siteName,
                    OMCName = siteName,
                    OfficeId = officeId,
                    Region = region,
                    SubRegion = subRegion,
                    Latitude = latitude,
                    Longitude = longitude,
                    Street = region,
                    City = subRegion,
                    AddressRegion = region,
                    AddressDetails = string.Empty,
                    SiteType = siteType
                }, cancellationToken);

                if (createResult.IsSuccess)
                {
                    imported++;
                }
                else
                {
                    skipped++;
                    errors.Add($"Row {rowIndex}: {createResult.Error}");
                }
            }

            var importResult = new ImportSiteDataResult
            {
                ImportedCount = imported,
                SkippedCount = skipped,
                Errors = errors
            };

            var strictFailure = ImportGuardrails.EnforceSkipInvalidRows(importResult, options);
            if (strictFailure is not null)
                return strictFailure;

            return Result.Success(importResult);
        }
        catch (Exception ex)
        {
            return Result.Failure<ImportSiteDataResult>($"Failed to import site data: {ex.Message}");
        }
    }

    private static Dictionary<string, int> BuildColumnMap(IXLRow headerRow)
    {
        var map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var cell in headerRow.CellsUsed())
        {
            var header = cell.GetString().Trim();
            if (!string.IsNullOrWhiteSpace(header))
                map[header] = cell.Address.ColumnNumber;
        }

        return map;
    }

    private static string GetCell(IXLRow row, Dictionary<string, int> columnMap, string column)
    {
        return columnMap.TryGetValue(column, out var col)
            ? row.Cell(col).GetString().Trim()
            : string.Empty;
    }

    private static bool TryParseSiteType(string value, out SiteType siteType)
    {
        if (Enum.TryParse<SiteType>(value.Replace(" ", string.Empty), true, out siteType))
            return true;

        if (int.TryParse(value, out var raw) && Enum.IsDefined(typeof(SiteType), raw))
        {
            siteType = (SiteType)raw;
            return true;
        }

        siteType = default;
        return false;
    }
}
