using ClosedXML.Excel;
using FluentValidation;
using MediatR;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Sites;
using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Application.Commands.Imports.ImportPanoramaEvidence;

public record ImportPanoramaEvidenceCommand : ICommand<ImportSiteDataResult>
{
    public Guid VisitId { get; init; }
    public byte[] FileContent { get; init; } = Array.Empty<byte>();
}

public class ImportPanoramaEvidenceCommandValidator : AbstractValidator<ImportPanoramaEvidenceCommand>
{
    public ImportPanoramaEvidenceCommandValidator()
    {
        RuleFor(x => x.VisitId)
            .NotEmpty();

        RuleFor(x => x.FileContent)
            .NotNull()
            .Must(x => x.Length > 0)
            .WithMessage("Excel file content is required.");
    }
}

public sealed class ImportPanoramaEvidenceCommandHandler : IRequestHandler<ImportPanoramaEvidenceCommand, Result<ImportSiteDataResult>>
{
    private readonly IVisitRepository _visitRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ImportPanoramaEvidenceCommandHandler(IVisitRepository visitRepository, IUnitOfWork unitOfWork)
    {
        _visitRepository = visitRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ImportSiteDataResult>> Handle(ImportPanoramaEvidenceCommand request, CancellationToken cancellationToken)
    {
        var visit = await _visitRepository.GetByIdAsync(request.VisitId, cancellationToken);
        if (visit is null)
            return Result.Failure<ImportSiteDataResult>("Visit not found.");

        using var stream = new MemoryStream(request.FileContent);
        using var workbook = new XLWorkbook(stream);

        var result = new ImportSiteDataResult();
        var panoramaSheet = workbook.Worksheets.FirstOrDefault(w =>
            string.Equals(w.Name.Trim(), "Panorama", StringComparison.OrdinalIgnoreCase));
        var towerSheet = workbook.Worksheets.FirstOrDefault(w =>
            string.Equals(w.Name.Trim(), "Tower Panorama", StringComparison.OrdinalIgnoreCase));

        if (panoramaSheet is null && towerSheet is null)
            return Result.Failure<ImportSiteDataResult>("Sheets 'Panorama' and 'Tower Panorama' were not found.");

        if (panoramaSheet is not null)
            ImportSheet(visit, panoramaSheet, PhotoCategory.ShelterInside, "panorama", result);

        if (towerSheet is not null)
            ImportSheet(visit, towerSheet, PhotoCategory.Tower, "tower-panorama", result);

        if (result.ImportedCount > 0)
        {
            await _visitRepository.UpdateAsync(visit, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result.Success(result);
    }

    private static void ImportSheet(
        Visit visit,
        IXLWorksheet worksheet,
        PhotoCategory category,
        string filePrefix,
        ImportSiteDataResult result)
    {
        var importedFromSheet = 0;
        var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;
        var fallbackTitle = worksheet.Row(1).CellsUsed().Select(c => c.GetString().Trim()).FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
        if (string.IsNullOrWhiteSpace(fallbackTitle))
            fallbackTitle = "Panorama";

        for (var rowNumber = 2; rowNumber <= lastRow; rowNumber++)
        {
            var row = worksheet.Row(rowNumber);
            if (row.IsEmpty())
                continue;

            var values = row.CellsUsed()
                .Select(c => c.GetString().Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            if (values.Count == 0)
                continue;

            for (var valueIndex = 0; valueIndex < values.Count; valueIndex++)
            {
                var token = values[valueIndex];
                if (token.Equals("N/A", StringComparison.OrdinalIgnoreCase) ||
                    token.Equals("NA", StringComparison.OrdinalIgnoreCase) ||
                    token.Equals("-", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                try
                {
                    var photo = VisitPhoto.Create(
                        visit.Id,
                        PhotoType.During,
                        category,
                        token,
                        $"{visit.VisitNumber}-{filePrefix}-{rowNumber}-{valueIndex + 1}.jpg",
                        $"import://{filePrefix}/{rowNumber}/{valueIndex + 1}");

                    visit.AddPhoto(photo);
                    importedFromSheet++;
                    result.ImportedCount++;
                }
                catch (Exception ex)
                {
                    result.SkippedCount++;
                    result.Errors.Add($"Sheet '{worksheet.Name}' row {rowNumber}: {ex.Message}");
                }
            }
        }

        if (importedFromSheet == 0)
        {
            result.SkippedCount++;
            result.Errors.Add($"Sheet '{worksheet.Name}' did not contain importable panorama rows. Header: {fallbackTitle}");
        }
    }
}
