using ClosedXML.Excel;
using FluentValidation;
using MediatR;
using TowerOps.Application.Commands.Imports;
using TowerOps.Application.Common;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Application.DTOs.Sites;
using TowerOps.Domain.Entities.Sites;
using TowerOps.Domain.Interfaces.Repositories;

namespace TowerOps.Application.Commands.Imports.ImportRFStatus;

public record ImportRFStatusCommand : ICommand<ImportSiteDataResult>
{
    public byte[] FileContent { get; init; } = Array.Empty<byte>();
}

public class ImportRFStatusCommandValidator : AbstractValidator<ImportRFStatusCommand>
{
    public ImportRFStatusCommandValidator()
    {
        RuleFor(x => x.FileContent)
            .NotNull()
            .Must(x => x.Length > 0)
            .WithMessage("Excel file content is required.");
    }
}

public sealed class ImportRFStatusCommandHandler : IRequestHandler<ImportRFStatusCommand, Result<ImportSiteDataResult>>
{
    private readonly ISiteRepository _siteRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISystemSettingsService? _systemSettingsService;

    public ImportRFStatusCommandHandler(ISiteRepository siteRepository, IUnitOfWork unitOfWork)
        : this(siteRepository, unitOfWork, null)
    {
    }

    public ImportRFStatusCommandHandler(
        ISiteRepository siteRepository,
        IUnitOfWork unitOfWork,
        ISystemSettingsService? systemSettingsService)
    {
        _siteRepository = siteRepository;
        _unitOfWork = unitOfWork;
        _systemSettingsService = systemSettingsService;
    }

    public async Task<Result<ImportSiteDataResult>> Handle(ImportRFStatusCommand request, CancellationToken cancellationToken)
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

        var worksheet = ImportExcelSupport.FindWorksheet(workbook, "RF Status");
        if (worksheet is null)
            return Result.Failure<ImportSiteDataResult>("Sheet 'RF Status' was not found.");

        var siteLookup = ImportExcelSupport.BuildSiteIdLookup(await _siteRepository.GetAllAsNoTrackingAsync(cancellationToken));
        var trackedSites = new Dictionary<Guid, Site>();

        var columnMap = ImportExcelSupport.BuildColumnMap(worksheet.Row(1));
        var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;

        for (var rowNumber = 2; rowNumber <= lastRow; rowNumber++)
        {
            var row = worksheet.Row(rowNumber);
            if (row.IsEmpty())
                continue;

            var siteCode = ImportExcelSupport.GetCellText(row, columnMap, "Site code", "Site Code", "Short Code");
            var key = ImportExcelSupport.NormalizeSiteKey(siteCode);
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

            var total = ImportExcelSupport.ParseInt(ImportExcelSupport.GetCellText(row, columnMap, "Total RF Count", "Total RF", "Total")) ?? 0;
            var onTower = ImportExcelSupport.ParseInt(ImportExcelSupport.GetCellText(row, columnMap, "RF On Tower Count", "RF on tower", "RFOnTower")) ?? 0;
            var onGround = ImportExcelSupport.ParseInt(ImportExcelSupport.GetCellText(row, columnMap, "RF On Ground Count", "RF on ground", "RFOnGround")) ?? 0;
            var sectorCarry = ImportExcelSupport.ParseInt(ImportExcelSupport.GetCellText(row, columnMap, "RF Sector Carry Count", "RFSectorCarry")) ?? 0;
            var bandTower = ImportExcelSupport.GetCellText(row, columnMap, "Band For RF On Tower", "Band Tower");
            var bandGround = ImportExcelSupport.GetCellText(row, columnMap, "Band For RF On Ground", "Band Ground");
            var notes = ImportExcelSupport.GetCellText(row, columnMap, "comment", "Notes");

            site.SetRFStatus(SiteRFStatus.Create(total, onTower, onGround, sectorCarry, bandTower, bandGround, notes));
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
}
