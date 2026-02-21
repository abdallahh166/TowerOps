using ClosedXML.Excel;
using FluentValidation;
using MediatR;
using TelecomPm.Application.Services.ExcelParsers;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Sites;
using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Application.Commands.Imports.ImportAlarmCapture;

public record ImportAlarmCaptureCommand : ICommand<ImportSiteDataResult>
{
    public Guid VisitId { get; init; }
    public byte[] FileContent { get; init; } = Array.Empty<byte>();
}

public class ImportAlarmCaptureCommandValidator : AbstractValidator<ImportAlarmCaptureCommand>
{
    public ImportAlarmCaptureCommandValidator()
    {
        RuleFor(x => x.VisitId).NotEmpty();

        RuleFor(x => x.FileContent)
            .NotNull()
            .Must(x => x.Length > 0)
            .WithMessage("Excel file content is required.");
    }
}

public sealed class ImportAlarmCaptureCommandHandler : IRequestHandler<ImportAlarmCaptureCommand, Result<ImportSiteDataResult>>
{
    private readonly IVisitRepository _visitRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ImportAlarmCaptureCommandHandler(IVisitRepository visitRepository, IUnitOfWork unitOfWork)
    {
        _visitRepository = visitRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ImportSiteDataResult>> Handle(ImportAlarmCaptureCommand request, CancellationToken cancellationToken)
    {
        var visit = await _visitRepository.GetByIdAsync(request.VisitId, cancellationToken);
        if (visit is null)
            return Result.Failure<ImportSiteDataResult>("Visit not found.");

        using var stream = new MemoryStream(request.FileContent);
        using var workbook = new XLWorkbook(stream);

        var worksheet = workbook.Worksheets.FirstOrDefault(w =>
            string.Equals(w.Name.Trim(), "alarms capture", StringComparison.OrdinalIgnoreCase));
        if (worksheet is null)
            return Result.Failure<ImportSiteDataResult>("Sheet 'alarms capture' was not found.");

        var result = new ImportSiteDataResult();
        var columnMap = BuildColumnMap(worksheet.Row(1));
        var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;
        var importedFromSheet = 0;

        for (var rowNumber = 2; rowNumber <= lastRow; rowNumber++)
        {
            var row = worksheet.Row(rowNumber);
            if (row.IsEmpty())
                continue;

            importedFromSheet += TryAddAlarmCapture(visit, row, rowNumber, result, columnMap, new[] { "BTS capture ( Alarms )", "BTS capture", "BTS" }, PhotoCategory.BTS, "bts-alarm");
            importedFromSheet += TryAddAlarmCapture(visit, row, rowNumber, result, columnMap, new[] { "3G capture", "3G" }, PhotoCategory.NodeB, "3g-alarm");
            importedFromSheet += TryAddAlarmCapture(visit, row, rowNumber, result, columnMap, new[] { "MW capture", "MW" }, PhotoCategory.MW, "mw-alarm");
        }

        if (importedFromSheet == 0)
        {
            result.SkippedCount++;
            result.Errors.Add("No alarm capture evidence rows were imported.");
            return Result.Success(result);
        }

        await _visitRepository.UpdateAsync(visit, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(result);
    }

    private static int TryAddAlarmCapture(
        Visit visit,
        IXLRow row,
        int rowNumber,
        ImportSiteDataResult result,
        Dictionary<string, int> columnMap,
        IEnumerable<string> aliases,
        PhotoCategory category,
        string fileToken)
    {
        var value = GetCellText(row, columnMap, aliases);
        var capture = BooleanTextParser.ParseNullable(value);
        if (capture != true)
            return 0;

        try
        {
            var photo = VisitPhoto.Create(
                visit.Id,
                PhotoType.During,
                category,
                $"Imported {category} alarm capture",
                $"{visit.VisitNumber}-{fileToken}-{rowNumber}.jpg",
                $"import://alarms/{fileToken}/{rowNumber}");

            visit.AddPhoto(photo);
            result.ImportedCount++;
            return 1;
        }
        catch (Exception ex)
        {
            result.SkippedCount++;
            result.Errors.Add($"Row {rowNumber}: {ex.Message}");
            return 0;
        }
    }

    private static Dictionary<string, int> BuildColumnMap(IXLRow headerRow)
    {
        var map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var cell in headerRow.CellsUsed())
        {
            var header = cell.GetString().Trim();
            if (!string.IsNullOrWhiteSpace(header) && !map.ContainsKey(header))
                map[header] = cell.Address.ColumnNumber;
        }

        return map;
    }

    private static string GetCellText(IXLRow row, Dictionary<string, int> columnMap, IEnumerable<string> aliases)
    {
        foreach (var alias in aliases)
        {
            if (columnMap.TryGetValue(alias, out var col))
                return row.Cell(col).GetString().Trim();
        }

        return string.Empty;
    }
}
