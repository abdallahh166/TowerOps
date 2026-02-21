using ClosedXML.Excel;
using FluentValidation;
using MediatR;
using TelecomPM.Application.Commands.Imports;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Sites;
using TelecomPM.Domain.Entities.Sites;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Application.Commands.Imports.ImportSiteSharingData;

public record ImportSiteSharingDataCommand : ICommand<ImportSiteDataResult>
{
    public byte[] FileContent { get; init; } = Array.Empty<byte>();
}

public class ImportSiteSharingDataCommandValidator : AbstractValidator<ImportSiteSharingDataCommand>
{
    public ImportSiteSharingDataCommandValidator()
    {
        RuleFor(x => x.FileContent)
            .NotNull()
            .Must(x => x.Length > 0)
            .WithMessage("Excel file content is required.");
    }
}

public sealed class ImportSiteSharingDataCommandHandler : IRequestHandler<ImportSiteSharingDataCommand, Result<ImportSiteDataResult>>
{
    private readonly ISiteRepository _siteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ImportSiteSharingDataCommandHandler(ISiteRepository siteRepository, IUnitOfWork unitOfWork)
    {
        _siteRepository = siteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ImportSiteDataResult>> Handle(ImportSiteSharingDataCommand request, CancellationToken cancellationToken)
    {
        var result = new ImportSiteDataResult();

        using var stream = new MemoryStream(request.FileContent);
        using var workbook = new XLWorkbook(stream);

        var worksheet = ImportExcelSupport.FindWorksheet(workbook, "Site Sharing Data");
        if (worksheet is null)
            return Result.Failure<ImportSiteDataResult>("Sheet 'Site Sharing Data' was not found.");

        var siteLookup = ImportExcelSupport.BuildSiteIdLookup(await _siteRepository.GetAllAsNoTrackingAsync(cancellationToken));
        var trackedSites = new Dictionary<Guid, Site>();

        var columnMap = ImportExcelSupport.BuildColumnMap(worksheet.Row(1));
        var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;

        for (var rowNumber = 2; rowNumber <= lastRow; rowNumber++)
        {
            var row = worksheet.Row(rowNumber);
            if (row.IsEmpty())
                continue;

            var shortCode = ImportExcelSupport.GetCellText(row, columnMap, "Short Code", "ShortCode", "Site code");
            var key = ImportExcelSupport.NormalizeSiteKey(shortCode);

            if (string.IsNullOrWhiteSpace(key) || !siteLookup.TryGetValue(key, out var siteId))
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

            try
            {
                var sharing = SiteSharing.Create(site.Id);

                var host = ImportExcelSupport.GetCellText(row, columnMap, "Site Host", "Host Operator");
                var guestsRaw = ImportExcelSupport.GetCellText(row, columnMap, "Site Guests");
                var guests = ImportExcelSupport.ParseGuests(guestsRaw);

                if (!ImportExcelSupport.IsBlankOrNa(host) || guests.Count > 0)
                    sharing.EnableSharing(ImportExcelSupport.IsBlankOrNa(host) ? "Unknown" : host, guests);

                sharing.SetHostSiteCode(ImportExcelSupport.GetCellText(row, columnMap, "Host Code"));
                sharing.SetTxEnclosureType(ImportExcelSupport.GetCellText(row, columnMap, "TX Enclosure"));

                AddAntennaPositions(row, columnMap, sharing, "Radio", 8);
                AddAntennaPositions(row, columnMap, sharing, "TX", 9);

                site.SetSharingInfo(sharing);
                result.ImportedCount++;
            }
            catch (Exception ex)
            {
                result.SkippedCount++;
                result.Errors.Add($"Row {rowNumber}: {ex.Message}");
            }
        }

        if (result.ImportedCount > 0)
            await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(result);
    }

    private static void AddAntennaPositions(
        IXLRow row,
        Dictionary<string, int> columnMap,
        SiteSharing sharing,
        string category,
        int maxIndex)
    {
        for (var i = 1; i <= maxIndex; i++)
        {
            var azimuthRaw = ImportExcelSupport.GetCellText(row, columnMap, $"{category} Azimuth {i}");
            var hbaRaw = ImportExcelSupport.GetCellText(row, columnMap, $"{category} HBA {i}");

            var azimuth = ImportExcelSupport.ParseDecimal(azimuthRaw);
            var hba = ImportExcelSupport.ParseDecimal(hbaRaw);

            if (!azimuth.HasValue || !hba.HasValue)
                continue;

            var position = SharedAntennaPosition.Create(sharing.Id, category, i, azimuth.Value, hba.Value);
            sharing.AddAntennaPosition(position);
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
}
