using ClosedXML.Excel;
using FluentValidation;
using MediatR;
using TelecomPM.Application.Commands.Imports;
using TelecomPM.Application.Common;
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Application.DTOs.Sites;
using TelecomPM.Domain.Entities.UnusedAssets;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Application.Commands.Imports.ImportUnusedAssets;

public record ImportUnusedAssetsCommand : ICommand<ImportSiteDataResult>
{
    public Guid VisitId { get; init; }
    public byte[] FileContent { get; init; } = Array.Empty<byte>();
}

public sealed class ImportUnusedAssetsCommandValidator : AbstractValidator<ImportUnusedAssetsCommand>
{
    public ImportUnusedAssetsCommandValidator()
    {
        RuleFor(x => x.VisitId).NotEmpty();
        RuleFor(x => x.FileContent)
            .NotNull()
            .Must(x => x.Length > 0)
            .WithMessage("Excel file content is required.");
    }
}

public sealed class ImportUnusedAssetsCommandHandler : IRequestHandler<ImportUnusedAssetsCommand, Result<ImportSiteDataResult>>
{
    private readonly IVisitRepository _visitRepository;
    private readonly IUnusedAssetRepository _unusedAssetRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISystemSettingsService? _systemSettingsService;

    public ImportUnusedAssetsCommandHandler(
        IVisitRepository visitRepository,
        IUnusedAssetRepository unusedAssetRepository,
        IUnitOfWork unitOfWork)
        : this(visitRepository, unusedAssetRepository, unitOfWork, null)
    {
    }

    public ImportUnusedAssetsCommandHandler(
        IVisitRepository visitRepository,
        IUnusedAssetRepository unusedAssetRepository,
        IUnitOfWork unitOfWork,
        ISystemSettingsService? systemSettingsService)
    {
        _visitRepository = visitRepository;
        _unusedAssetRepository = unusedAssetRepository;
        _unitOfWork = unitOfWork;
        _systemSettingsService = systemSettingsService;
    }

    public async Task<Result<ImportSiteDataResult>> Handle(ImportUnusedAssetsCommand request, CancellationToken cancellationToken)
    {
        var options = await ImportGuardrails.ResolveOptionsAsync(_systemSettingsService, cancellationToken);
        var fileValidationError = ImportGuardrails.ValidateExcelPayload(request.FileContent, options);
        if (fileValidationError is not null)
            return Result.Failure<ImportSiteDataResult>(fileValidationError);

        var visit = await _visitRepository.GetByIdAsNoTrackingAsync(request.VisitId, cancellationToken);
        if (visit is null)
            return Result.Failure<ImportSiteDataResult>("Visit not found.");

        using var stream = new MemoryStream(request.FileContent);
        using var workbook = new XLWorkbook(stream);
        var rowLimitFailure = ImportGuardrails.ValidateRowLimit(
            ImportGuardrails.CountNonEmptyDataRows(workbook),
            options);
        if (rowLimitFailure is not null)
            return rowLimitFailure;

        var worksheet = workbook.Worksheets.FirstOrDefault(w =>
            string.Equals(w.Name.Trim(), "unused assets", StringComparison.OrdinalIgnoreCase));
        if (worksheet is null)
            return Result.Failure<ImportSiteDataResult>("Sheet 'unused assets' was not found.");

        var result = new ImportSiteDataResult();
        var columnMap = ImportExcelSupport.BuildColumnMap(worksheet.Row(1));
        var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;

        for (var rowNumber = 2; rowNumber <= lastRow; rowNumber++)
        {
            var row = worksheet.Row(rowNumber);
            if (row.IsEmpty())
                continue;

            var assetName = ResolveAssetName(row, columnMap);
            if (string.IsNullOrWhiteSpace(assetName))
            {
                result.SkippedCount++;
                result.Errors.Add($"Row {rowNumber}: asset name is missing.");
                continue;
            }

            var quantityRaw = ImportExcelSupport.GetCellText(row, columnMap, "Quantity", "Qty", "Count");
            var quantity = ImportExcelSupport.ParseDecimal(quantityRaw);
            var resolvedQuantity = quantity.HasValue && quantity.Value > 0 ? quantity.Value : 1m;
            var notes = ImportExcelSupport.GetCellText(row, columnMap, "Notes", "Comment", "Remarks");

            try
            {
                var unusedAsset = UnusedAsset.Create(
                    visit.SiteId,
                    visit.Id,
                    assetName,
                    resolvedQuantity,
                    null,
                    DateTime.UtcNow,
                    ImportExcelSupport.IsBlankOrNa(notes) ? null : notes);

                await _unusedAssetRepository.AddAsync(unusedAsset, cancellationToken);
                result.ImportedCount++;
            }
            catch (Exception ex)
            {
                result.SkippedCount++;
                result.Errors.Add($"Row {rowNumber}: {ex.Message}");
            }
        }

        if (result.ImportedCount == 0)
        {
            result.SkippedCount++;
            result.Errors.Add("No importable unused asset rows found.");
            var strictFailure = ImportGuardrails.EnforceSkipInvalidRows(result, options);
            if (strictFailure is not null)
                return strictFailure;

            return Result.Success(result);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var failure = ImportGuardrails.EnforceSkipInvalidRows(result, options);
        if (failure is not null)
            return failure;

        return Result.Success(result);
    }

    private static string ResolveAssetName(IXLRow row, Dictionary<string, int> columnMap)
    {
        var namedColumnValue = ImportExcelSupport.GetCellText(
            row,
            columnMap,
            "Asset",
            "Unused Asset",
            "Material",
            "Item",
            "Item Name",
            "Description");

        if (!ImportExcelSupport.IsBlankOrNa(namedColumnValue))
            return namedColumnValue;

        var values = row.CellsUsed()
            .Select(c => c.GetString().Trim())
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .ToList();

        foreach (var value in values)
        {
            if (decimal.TryParse(value, out _))
                continue;

            return value;
        }

        return string.Empty;
    }
}
