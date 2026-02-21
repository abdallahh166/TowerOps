using ClosedXML.Excel;
using FluentValidation;
using MediatR;
using TelecomPM.Application.Commands.Imports;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Sites;
using TelecomPM.Domain.Entities.Sites;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Application.Commands.Imports.ImportSiteAssets;

public record ImportSiteAssetsCommand : ICommand<ImportSiteDataResult>
{
    public byte[] FileContent { get; init; } = Array.Empty<byte>();
}

public class ImportSiteAssetsCommandValidator : AbstractValidator<ImportSiteAssetsCommand>
{
    public ImportSiteAssetsCommandValidator()
    {
        RuleFor(x => x.FileContent)
            .NotNull()
            .Must(x => x.Length > 0)
            .WithMessage("Excel file content is required.");
    }
}

public sealed class ImportSiteAssetsCommandHandler : IRequestHandler<ImportSiteAssetsCommand, Result<ImportSiteDataResult>>
{
    private readonly ISiteRepository _siteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ImportSiteAssetsCommandHandler(ISiteRepository siteRepository, IUnitOfWork unitOfWork)
    {
        _siteRepository = siteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ImportSiteDataResult>> Handle(ImportSiteAssetsCommand request, CancellationToken cancellationToken)
    {
        var result = new ImportSiteDataResult();
        var trackedSites = new Dictionary<Guid, Site>();

        using var stream = new MemoryStream(request.FileContent);
        using var workbook = new XLWorkbook(stream);

        var worksheet = ImportExcelSupport.FindWorksheet(workbook, "Site Assets Data Count");
        if (worksheet is null)
            return Result.Failure<ImportSiteDataResult>("Sheet 'Site Assets Data Count' was not found.");

        var siteLookup = ImportExcelSupport.BuildSiteIdLookup(await _siteRepository.GetAllAsNoTrackingAsync(cancellationToken));
        var columnMap = ImportExcelSupport.BuildColumnMap(worksheet.Row(1));

        var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;
        for (var rowNumber = 2; rowNumber <= lastRow; rowNumber++)
        {
            var row = worksheet.Row(rowNumber);
            if (row.IsEmpty())
                continue;

            var siteCode = ImportExcelSupport.GetCellText(row, columnMap, "ShortCode", "Site Code", "SiteCode", "Short Code");
            var key = ImportExcelSupport.NormalizeSiteKey(siteCode);
            if (string.IsNullOrWhiteSpace(key))
            {
                result.SkippedCount++;
                result.Errors.Add($"Row {rowNumber}: Site code is missing.");
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

            var subcontractor = ImportExcelSupport.GetCellText(row, columnMap, "Subcontractor");
            var maintenanceArea = ImportExcelSupport.GetCellText(row, columnMap, "Maintenance Area");
            if (!ImportExcelSupport.IsBlankOrNa(subcontractor) || !ImportExcelSupport.IsBlankOrNa(maintenanceArea))
            {
                site.SetContractorInfo(
                    ImportExcelSupport.IsBlankOrNa(subcontractor) ? site.Subcontractor : subcontractor,
                    ImportExcelSupport.IsBlankOrNa(maintenanceArea) ? site.MaintenanceArea : maintenanceArea);
            }

            var zteMonitoring = ImportExcelSupport.GetCellText(row, columnMap, "ZTE Remote Monitoring System");
            var generalNotes = ImportExcelSupport.GetCellText(row, columnMap, "General Data");
            if (!ImportExcelSupport.IsBlankOrNa(zteMonitoring) || !ImportExcelSupport.IsBlankOrNa(generalNotes))
            {
                site.SetMonitoringInfo(
                    ImportExcelSupport.IsBlankOrNa(zteMonitoring) ? null : zteMonitoring,
                    ImportExcelSupport.IsBlankOrNa(generalNotes) ? null : generalNotes);
            }

            var statusText = ImportExcelSupport.GetCellText(row, columnMap, "On / Off  Air", "Status", "On/Off Air");
            var parsedStatus = ImportExcelSupport.ParseSiteStatus(statusText);
            if (parsedStatus.HasValue && parsedStatus.Value != site.Status)
            {
                site.UpdateStatus(parsedStatus.Value);
            }

            var enclosureRaw = ImportExcelSupport.GetCellText(row, columnMap, "Cooling System");
            site.SetEnclosureInfo(ImportExcelSupport.ParseEnclosureType(enclosureRaw), ImportExcelSupport.IsBlankOrNa(enclosureRaw) ? null : enclosureRaw);

            result.ImportedCount++;
        }

        if (result.ImportedCount > 0)
            await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(result);
    }

    private async Task<Site?> GetTrackedSiteAsync(
        Guid siteId,
        Dictionary<Guid, Site> trackedSites,
        CancellationToken cancellationToken)
    {
        if (trackedSites.TryGetValue(siteId, out var cached))
            return cached;

        var site = await _siteRepository.GetByIdAsync(siteId, cancellationToken);
        if (site is not null)
            trackedSites[siteId] = site;

        return site;
    }
}
