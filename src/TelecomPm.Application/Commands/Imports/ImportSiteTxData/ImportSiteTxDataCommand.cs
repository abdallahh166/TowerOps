using ClosedXML.Excel;
using FluentValidation;
using MediatR;
using TelecomPM.Application.Commands.Imports;
using TelecomPM.Application.Common;
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Application.DTOs.Sites;
using TelecomPM.Domain.Entities.Sites;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Application.Commands.Imports.ImportSiteTxData;

public record ImportSiteTxDataCommand : ICommand<ImportSiteDataResult>
{
    public byte[] FileContent { get; init; } = Array.Empty<byte>();
}

public class ImportSiteTxDataCommandValidator : AbstractValidator<ImportSiteTxDataCommand>
{
    public ImportSiteTxDataCommandValidator()
    {
        RuleFor(x => x.FileContent)
            .NotNull()
            .Must(x => x.Length > 0)
            .WithMessage("Excel file content is required.");
    }
}

public sealed class ImportSiteTxDataCommandHandler : IRequestHandler<ImportSiteTxDataCommand, Result<ImportSiteDataResult>>
{
    private readonly ISiteRepository _siteRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISystemSettingsService? _systemSettingsService;

    public ImportSiteTxDataCommandHandler(ISiteRepository siteRepository, IUnitOfWork unitOfWork)
        : this(siteRepository, unitOfWork, null)
    {
    }

    public ImportSiteTxDataCommandHandler(
        ISiteRepository siteRepository,
        IUnitOfWork unitOfWork,
        ISystemSettingsService? systemSettingsService)
    {
        _siteRepository = siteRepository;
        _unitOfWork = unitOfWork;
        _systemSettingsService = systemSettingsService;
    }

    public async Task<Result<ImportSiteDataResult>> Handle(ImportSiteTxDataCommand request, CancellationToken cancellationToken)
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

        var worksheet = ImportExcelSupport.FindWorksheet(workbook, "Site TX Data");
        if (worksheet is null)
            return Result.Failure<ImportSiteDataResult>("Sheet 'Site TX Data' was not found.");

        var siteLookup = ImportExcelSupport.BuildSiteIdLookup(await _siteRepository.GetAllAsNoTrackingAsync(cancellationToken));
        var trackedSites = new Dictionary<Guid, Site>();

        var linksPerSite = new Dictionary<Guid, List<(MWLink Link, string? NodalRaw)>>();

        var columnMap = ImportExcelSupport.BuildColumnMap(worksheet.Row(1));
        var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;

        for (var rowNumber = 2; rowNumber <= lastRow; rowNumber++)
        {
            var row = worksheet.Row(rowNumber);
            if (row.IsEmpty())
                continue;

            var shortCode = ImportExcelSupport.GetCellText(row, columnMap, "Short Code", "Site Code");
            var key = ImportExcelSupport.NormalizeSiteKey(shortCode);
            if (string.IsNullOrWhiteSpace(key) || !siteLookup.TryGetValue(key, out var siteId))
            {
                result.SkippedCount++;
                result.Errors.Add($"Row {rowNumber}: site '{key}' not found.");
                continue;
            }

            var directionName = ImportExcelSupport.GetCellText(row, columnMap, "Directions Site Name");
            var directionCode = ImportExcelSupport.GetCellText(row, columnMap, "Directions Site Code");
            var band = ImportExcelSupport.GetCellText(row, columnMap, "Band");
            var linkModel = ImportExcelSupport.GetCellText(row, columnMap, "Link Model", "ODU Model");
            var dishSizeMeters = ImportExcelSupport.ParseDecimal(ImportExcelSupport.GetCellText(row, columnMap, "Antenna Diameter [m]")) ?? 0.6m;
            var configuration = ImportExcelSupport.GetCellText(row, columnMap, "Configuration");

            var link = MWLink.Create(
                string.IsNullOrWhiteSpace(directionName) ? directionCode : directionName,
                string.IsNullOrWhiteSpace(directionCode) ? directionName : directionCode,
                string.IsNullOrWhiteSpace(band) ? "Unknown" : band,
                (int)Math.Round(dishSizeMeters * 100),
                string.IsNullOrWhiteSpace(linkModel) ? "Unknown" : linkModel);

            link.SetRadioDetails(
                ImportExcelSupport.ParseDecimal(ImportExcelSupport.GetCellText(row, columnMap, "Tx Frequency [KHz]")),
                ImportExcelSupport.ParseDecimal(ImportExcelSupport.GetCellText(row, columnMap, "Rx Frequency [KHz]")),
                ImportExcelSupport.ParseDecimal(ImportExcelSupport.GetCellText(row, columnMap, "Tx Power [dBm]")),
                ImportExcelSupport.ParseDecimal(ImportExcelSupport.GetCellText(row, columnMap, "Rx Power [dBm]")),
                ImportExcelSupport.ParseDecimal(ImportExcelSupport.GetCellText(row, columnMap, "Capacity [Mb/s]")),
                ImportExcelSupport.GetCellText(row, columnMap, "Modulation"),
                configuration,
                ImportExcelSupport.GetCellText(row, columnMap, "Polarization"));

            link.SetAssetDetails(
                ImportExcelSupport.GetCellText(row, columnMap, "IP Address"),
                ImportExcelSupport.GetCellText(row, columnMap, "ODU S/N"),
                ImportExcelSupport.GetCellText(row, columnMap, "Opposite site ODU S/N"),
                ImportExcelSupport.GetCellText(row, columnMap, "Antenna Reference"),
                ImportExcelSupport.ParseDecimal(ImportExcelSupport.GetCellText(row, columnMap, "TX Azimuth")),
                ImportExcelSupport.ParseDecimal(ImportExcelSupport.GetCellText(row, columnMap, "TX HBA")));

            if (!linksPerSite.TryGetValue(siteId, out var siteLinks))
            {
                siteLinks = new List<(MWLink, string?)>();
                linksPerSite[siteId] = siteLinks;
            }

            siteLinks.Add((link, configuration));
            result.ImportedCount++;
        }

        foreach (var (siteId, links) in linksPerSite)
        {
            var site = await GetTrackedSiteAsync(siteId, trackedSites, cancellationToken);
            if (site is null)
                continue;

            var existing = site.Transmission;
            var transmission = SiteTransmission.Create(
                site.Id,
                existing?.Type ?? TransmissionType.MW,
                existing?.Supplier ?? "Imported");

            transmission.SetEquipment(
                existing?.HasGPS ?? false,
                existing?.HasADM ?? false,
                existing?.HasSDH ?? false,
                existing?.HasEBand ?? false,
                existing?.HasALURouter ?? false);

            foreach (var (link, nodalRaw) in links)
            {
                transmission.AddMWLink(link);
                if (!ImportExcelSupport.IsBlankOrNa(nodalRaw))
                    transmission.SetNodalDegree(nodalRaw);
            }

            site.SetTransmission(transmission);
        }

        if (linksPerSite.Count > 0)
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
}
