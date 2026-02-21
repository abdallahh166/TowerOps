using ClosedXML.Excel;
using FluentValidation;
using MediatR;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Sites;
using TelecomPM.Domain.Entities.ChecklistTemplates;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Application.Commands.Imports.ImportChecklistTemplate;

public record ImportChecklistTemplateCommand : ICommand<ImportSiteDataResult>
{
    public byte[] FileContent { get; init; } = Array.Empty<byte>();
    public VisitType VisitType { get; init; }
    public string Version { get; init; } = string.Empty;
    public DateTime EffectiveFromUtc { get; init; } = DateTime.UtcNow;
    public string CreatedBy { get; init; } = string.Empty;
    public string? ChangeNotes { get; init; }
}

public class ImportChecklistTemplateCommandValidator : AbstractValidator<ImportChecklistTemplateCommand>
{
    public ImportChecklistTemplateCommandValidator()
    {
        RuleFor(x => x.FileContent)
            .NotNull()
            .Must(x => x.Length > 0)
            .WithMessage("Excel file content is required.");

        RuleFor(x => x.Version)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.CreatedBy)
            .NotEmpty()
            .MaximumLength(100);
    }
}

public sealed class ImportChecklistTemplateCommandHandler : IRequestHandler<ImportChecklistTemplateCommand, Result<ImportSiteDataResult>>
{
    private static readonly string[] MetadataPrefixes =
    {
        "IMPORTANT TIPS",
        "FILE VERSION",
        "DEPARTMENT",
        "CREATOR",
        "APPROVER",
        "DATE",
        "REVISION",
        "PURPOSE",
        "SCOPE",
        "PROCEDURE",
        "REFERENCE"
    };

    private readonly IChecklistTemplateRepository _checklistTemplateRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ImportChecklistTemplateCommandHandler(
        IChecklistTemplateRepository checklistTemplateRepository,
        IUnitOfWork unitOfWork)
    {
        _checklistTemplateRepository = checklistTemplateRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ImportSiteDataResult>> Handle(ImportChecklistTemplateCommand request, CancellationToken cancellationToken)
    {
        var existing = await _checklistTemplateRepository.GetByVisitTypeAsync(request.VisitType, cancellationToken);
        if (existing.Any(x => string.Equals(x.Version, request.Version, StringComparison.OrdinalIgnoreCase)))
            return Result.Failure<ImportSiteDataResult>($"Template version '{request.Version}' already exists for {request.VisitType}.");

        using var stream = new MemoryStream(request.FileContent);
        using var workbook = new XLWorkbook(stream);

        var result = new ImportSiteDataResult();
        var template = ChecklistTemplate.Create(
            request.VisitType,
            request.Version,
            request.EffectiveFromUtc,
            request.CreatedBy,
            request.ChangeNotes);

        if (!TryImportCommonChecklist(workbook, template, result))
        {
            if (request.VisitType == VisitType.Audit)
            {
                TryImportAuditMatrix(workbook, template, result);
            }
        }

        if (template.Items.Count == 0)
            return Result.Failure<ImportSiteDataResult>("No checklist items could be extracted from the workbook.");

        await _checklistTemplateRepository.AddAsync(template, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(result);
    }

    private static bool TryImportCommonChecklist(XLWorkbook workbook, ChecklistTemplate template, ImportSiteDataResult result)
    {
        var worksheet = workbook.Worksheets.FirstOrDefault(w =>
            string.Equals(w.Name.Trim(), "Common checklist", StringComparison.OrdinalIgnoreCase));

        if (worksheet is null)
        {
            result.Errors.Add("Sheet 'Common checklist' was not found.");
            return false;
        }

        var currentCategory = "General";
        var orderIndex = 1;
        var importedFromSheet = 0;
        var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;

        for (var rowNumber = 1; rowNumber <= lastRow; rowNumber++)
        {
            var row = worksheet.Row(rowNumber);
            if (row.IsEmpty())
                continue;

            var c1 = row.Cell(1).GetString().Trim();
            var c2 = row.Cell(2).GetString().Trim();
            var c3 = row.Cell(3).GetString().Trim();
            var c4 = row.Cell(4).GetString().Trim();
            var c5 = row.Cell(5).GetString().Trim();

            if (string.IsNullOrWhiteSpace(c1) && string.IsNullOrWhiteSpace(c2) && string.IsNullOrWhiteSpace(c3))
                continue;

            if (!string.IsNullOrWhiteSpace(c1) && string.IsNullOrWhiteSpace(c2) && string.IsNullOrWhiteSpace(c3))
            {
                currentCategory = c1;
                continue;
            }

            var candidateCategory = !string.IsNullOrWhiteSpace(c1) && !LooksLikeMetadata(c1)
                ? c1
                : currentCategory;

            var itemName = FirstNonEmpty(c2, c3);
            if (string.IsNullOrWhiteSpace(itemName) && c1.Contains("CHECK", StringComparison.OrdinalIgnoreCase))
                itemName = c1;

            if (string.IsNullOrWhiteSpace(itemName) || LooksLikeMetadata(itemName))
                continue;

            var isMandatory = ParseMandatory(c4, c5, itemName);
            var description = !string.IsNullOrWhiteSpace(c3) && !string.Equals(itemName, c3, StringComparison.Ordinal)
                ? c3
                : null;

            template.AddItem(
                string.IsNullOrWhiteSpace(candidateCategory) ? "General" : candidateCategory,
                itemName,
                description,
                isMandatory,
                orderIndex++);

            importedFromSheet++;
            result.ImportedCount++;
        }

        if (importedFromSheet == 0)
        {
            result.SkippedCount++;
            result.Errors.Add("Sheet 'Common checklist' did not contain importable checklist rows.");
        }

        return importedFromSheet > 0;
    }

    private static bool TryImportAuditMatrix(XLWorkbook workbook, ChecklistTemplate template, ImportSiteDataResult result)
    {
        var worksheet = workbook.Worksheets.FirstOrDefault(w =>
            string.Equals(w.Name.Trim(), "Audit matrix SQI", StringComparison.OrdinalIgnoreCase));

        if (worksheet is null)
        {
            result.Errors.Add("Sheet 'Audit matrix SQI' was not found.");
            return false;
        }

        var orderIndex = template.Items.Count + 1;
        var importedFromSheet = 0;
        var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;

        for (var rowNumber = 2; rowNumber <= lastRow; rowNumber++)
        {
            var row = worksheet.Row(rowNumber);
            if (row.IsEmpty())
                continue;

            var category = row.Cell(1).GetString().Trim();
            var itemName = row.Cell(2).GetString().Trim();

            if (string.IsNullOrWhiteSpace(itemName))
                continue;

            if (!string.IsNullOrWhiteSpace(category) && LooksLikeMetadata(category))
                continue;

            template.AddItem(
                string.IsNullOrWhiteSpace(category) ? "SQI" : category,
                itemName,
                null,
                true,
                orderIndex++);

            importedFromSheet++;
            result.ImportedCount++;
        }

        if (importedFromSheet == 0)
        {
            result.SkippedCount++;
            result.Errors.Add("Sheet 'Audit matrix SQI' did not contain importable checklist rows.");
        }

        return importedFromSheet > 0;
    }

    private static bool ParseMandatory(string? c4, string? c5, string itemName)
    {
        var token = FirstNonEmpty(c5, c4);
        if (!string.IsNullOrWhiteSpace(token))
        {
            if (token.Contains("OPTIONAL", StringComparison.OrdinalIgnoreCase))
                return false;

            if (token.Contains("MANDATORY", StringComparison.OrdinalIgnoreCase) || token.Equals("TRUE", StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return !itemName.Contains("OPTIONAL", StringComparison.OrdinalIgnoreCase);
    }

    private static bool LooksLikeMetadata(string value)
    {
        var normalized = value.Trim().ToUpperInvariant();
        return MetadataPrefixes.Any(prefix => normalized.StartsWith(prefix, StringComparison.Ordinal));
    }

    private static string FirstNonEmpty(params string?[] values)
    {
        foreach (var value in values)
        {
            if (!string.IsNullOrWhiteSpace(value))
                return value.Trim();
        }

        return string.Empty;
    }
}
