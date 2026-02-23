using ClosedXML.Excel;
using FluentValidation;
using MediatR;
using TelecomPM.Application.Common;
using TelecomPM.Domain.Entities.ChecklistTemplates;
using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Application.Commands.Reports.ExportChecklist;

public record ExportChecklistCommand : ICommand<byte[]>
{
    public Guid? VisitId { get; init; }
    public VisitType? VisitType { get; init; }
}

public class ExportChecklistCommandValidator : AbstractValidator<ExportChecklistCommand>
{
    public ExportChecklistCommandValidator()
    {
        RuleFor(x => x.VisitId)
            .Must(x => x == null || x != Guid.Empty)
            .WithMessage("VisitId must be a non-empty GUID.");
    }
}

public sealed class ExportChecklistCommandHandler : IRequestHandler<ExportChecklistCommand, Result<byte[]>>
{
    private readonly IVisitRepository _visitRepository;
    private readonly IChecklistTemplateRepository _checklistTemplateRepository;
    private readonly IUnusedAssetRepository? _unusedAssetRepository;

    public ExportChecklistCommandHandler(
        IVisitRepository visitRepository,
        IChecklistTemplateRepository checklistTemplateRepository)
        : this(visitRepository, checklistTemplateRepository, null)
    {
    }

    public ExportChecklistCommandHandler(
        IVisitRepository visitRepository,
        IChecklistTemplateRepository checklistTemplateRepository,
        IUnusedAssetRepository? unusedAssetRepository)
    {
        _visitRepository = visitRepository;
        _checklistTemplateRepository = checklistTemplateRepository;
        _unusedAssetRepository = unusedAssetRepository;
    }

    public async Task<Result<byte[]>> Handle(ExportChecklistCommand request, CancellationToken cancellationToken)
    {
        var visits = await LoadVisitsAsync(request.VisitId, cancellationToken);
        var templates = await LoadTemplatesAsync(request.VisitType, cancellationToken);
        var unusedAssets = await LoadUnusedAssetsAsync(visits, cancellationToken);

        using var workbook = new XLWorkbook();
        BuildSitesReadingSheet(workbook, visits);
        BuildCommonChecklistSheet(workbook, templates);
        BuildPanoramaSheet(workbook, "Panorama", visits, p => p.Category == PhotoCategory.ShelterInside || p.Category == PhotoCategory.ShelterOutside);
        BuildPanoramaSheet(workbook, "Tower Panorama", visits, p => p.Category == PhotoCategory.Tower);
        BuildBeforeAfterSheet(workbook, visits);
        BuildPendingReservesSheet(workbook, visits);
        BuildUnusedAssetsSheet(workbook, visits, unusedAssets);
        BuildAlarmsCaptureSheet(workbook, visits);
        BuildAuditMatrixSheet(workbook, templates);

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return Result.Success(stream.ToArray());
    }

    private async Task<IReadOnlyList<Visit>> LoadVisitsAsync(Guid? visitId, CancellationToken cancellationToken)
    {
        if (visitId.HasValue)
        {
            var visit = await _visitRepository.GetByIdAsNoTrackingAsync(visitId.Value, cancellationToken);
            return visit is null ? Array.Empty<Visit>() : new[] { visit };
        }

        var allVisits = await _visitRepository.GetAllAsNoTrackingAsync(cancellationToken);
        if (allVisits.Count == 0)
            return allVisits;

        var hydrated = new List<Visit>(allVisits.Count);
        foreach (var visit in allVisits)
        {
            var withDetails = await _visitRepository.GetByIdAsNoTrackingAsync(visit.Id, cancellationToken);
            if (withDetails is not null)
                hydrated.Add(withDetails);
        }

        return hydrated;
    }

    private async Task<IReadOnlyList<ChecklistTemplate>> LoadTemplatesAsync(VisitType? visitType, CancellationToken cancellationToken)
    {
        if (visitType.HasValue)
            return await _checklistTemplateRepository.GetByVisitTypeAsync(visitType.Value, cancellationToken);

        var unique = new Dictionary<Guid, ChecklistTemplate>();
        foreach (var type in Enum.GetValues<VisitType>())
        {
            var templates = await _checklistTemplateRepository.GetByVisitTypeAsync(type, cancellationToken);
            foreach (var template in templates)
            {
                if (!unique.ContainsKey(template.Id))
                    unique[template.Id] = template;
            }
        }

        return unique.Values.OrderByDescending(t => t.EffectiveFromUtc).ToList();
    }

    private async Task<IReadOnlyList<TelecomPM.Domain.Entities.UnusedAssets.UnusedAsset>> LoadUnusedAssetsAsync(
        IReadOnlyList<Visit> visits,
        CancellationToken cancellationToken)
    {
        if (_unusedAssetRepository is null || visits.Count == 0)
            return Array.Empty<TelecomPM.Domain.Entities.UnusedAssets.UnusedAsset>();

        var visitIds = visits.Select(v => v.Id).ToList();
        return await _unusedAssetRepository.GetByVisitIdsAsNoTrackingAsync(visitIds, cancellationToken);
    }

    private static void BuildSitesReadingSheet(XLWorkbook workbook, IReadOnlyList<Visit> visits)
    {
        var ws = workbook.AddWorksheet("site's reading");
        ws.Cell(1, 1).Value = "Visit Number";
        ws.Cell(1, 2).Value = "Site Name";
        ws.Cell(1, 3).Value = "Site Code";
        ws.Cell(1, 4).Value = "Date";
        ws.Cell(1, 5).Value = "Reading Type";
        ws.Cell(1, 6).Value = "Value";
        ws.Cell(1, 7).Value = "Unit";

        var row = 2;
        foreach (var visit in visits)
        {
            foreach (var reading in visit.Readings)
            {
                ws.Cell(row, 1).Value = visit.VisitNumber;
                ws.Cell(row, 2).Value = visit.SiteName;
                ws.Cell(row, 3).Value = visit.SiteCode;
                ws.Cell(row, 4).Value = visit.ScheduledDate;
                ws.Cell(row, 5).Value = reading.ReadingType;
                ws.Cell(row, 6).Value = reading.Value;
                ws.Cell(row, 7).Value = reading.Unit;
                row++;
            }
        }

        ws.Columns().AdjustToContents();
    }

    private static void BuildCommonChecklistSheet(XLWorkbook workbook, IReadOnlyList<ChecklistTemplate> templates)
    {
        var ws = workbook.AddWorksheet("Common checklist");
        ws.Cell(1, 1).Value = "Visit Type";
        ws.Cell(1, 2).Value = "Version";
        ws.Cell(1, 3).Value = "Category";
        ws.Cell(1, 4).Value = "Item Name";
        ws.Cell(1, 5).Value = "Mandatory";

        var row = 2;
        foreach (var template in templates)
        {
            foreach (var item in template.Items.OrderBy(i => i.OrderIndex))
            {
                ws.Cell(row, 1).Value = template.VisitType.ToString();
                ws.Cell(row, 2).Value = template.Version;
                ws.Cell(row, 3).Value = item.Category;
                ws.Cell(row, 4).Value = item.ItemName;
                ws.Cell(row, 5).Value = item.IsMandatory;
                row++;
            }
        }

        ws.Columns().AdjustToContents();
    }

    private static void BuildPanoramaSheet(XLWorkbook workbook, string sheetName, IReadOnlyList<Visit> visits, Func<VisitPhoto, bool> predicate)
    {
        var ws = workbook.AddWorksheet(sheetName);
        ws.Cell(1, 1).Value = "Visit Number";
        ws.Cell(1, 2).Value = "Site Code";
        ws.Cell(1, 3).Value = "Photo";
        ws.Cell(1, 4).Value = "Captured At";

        var row = 2;
        foreach (var visit in visits)
        {
            foreach (var photo in visit.Photos.Where(predicate))
            {
                ws.Cell(row, 1).Value = visit.VisitNumber;
                ws.Cell(row, 2).Value = visit.SiteCode;
                ws.Cell(row, 3).Value = photo.FileName;
                ws.Cell(row, 4).Value = photo.CapturedAtUtc ?? photo.CreatedAt;
                row++;
            }
        }

        ws.Columns().AdjustToContents();
    }

    private static void BuildBeforeAfterSheet(XLWorkbook workbook, IReadOnlyList<Visit> visits)
    {
        var ws = workbook.AddWorksheet("Before & after");
        ws.Cell(1, 1).Value = "Visit Number";
        ws.Cell(1, 2).Value = "Site Code";
        ws.Cell(1, 3).Value = "Before Photo";
        ws.Cell(1, 4).Value = "After Photo";

        var row = 2;
        foreach (var visit in visits)
        {
            var before = visit.Photos.FirstOrDefault(p => p.Type == PhotoType.Before)?.FileName;
            var after = visit.Photos.FirstOrDefault(p => p.Type == PhotoType.After)?.FileName;

            ws.Cell(row, 1).Value = visit.VisitNumber;
            ws.Cell(row, 2).Value = visit.SiteCode;
            ws.Cell(row, 3).Value = before;
            ws.Cell(row, 4).Value = after;
            row++;
        }

        ws.Columns().AdjustToContents();
    }

    private static void BuildPendingReservesSheet(XLWorkbook workbook, IReadOnlyList<Visit> visits)
    {
        var ws = workbook.AddWorksheet("Pending Res.");
        ws.Cell(1, 1).Value = "No.";
        ws.Cell(1, 2).Value = "Visit Number";
        ws.Cell(1, 3).Value = "Site Code";
        ws.Cell(1, 4).Value = "Issue";
        ws.Cell(1, 5).Value = "Status";

        var row = 2;
        var index = 1;
        foreach (var visit in visits)
        {
            foreach (var issue in visit.IssuesFound.Where(i => i.Status != IssueStatus.Resolved && i.Status != IssueStatus.Closed))
            {
                ws.Cell(row, 1).Value = index++;
                ws.Cell(row, 2).Value = visit.VisitNumber;
                ws.Cell(row, 3).Value = visit.SiteCode;
                ws.Cell(row, 4).Value = issue.Title;
                ws.Cell(row, 5).Value = "Pending";
                row++;
            }
        }

        ws.Columns().AdjustToContents();
    }

    private static void BuildUnusedAssetsSheet(
        XLWorkbook workbook,
        IReadOnlyList<Visit> visits,
        IReadOnlyList<TelecomPM.Domain.Entities.UnusedAssets.UnusedAsset> unusedAssets)
    {
        var ws = workbook.AddWorksheet("unused assets");
        ws.Cell(1, 1).Value = "No.";
        ws.Cell(1, 2).Value = "Visit Number";
        ws.Cell(1, 3).Value = "Site Code";
        ws.Cell(1, 4).Value = "Asset";
        ws.Cell(1, 5).Value = "Quantity";
        ws.Cell(1, 6).Value = "Notes";
        ws.Cell(1, 7).Value = "Recorded At";

        var row = 2;
        var index = 1;
        var visitLookup = visits.ToDictionary(v => v.Id);
        if (unusedAssets.Count > 0)
        {
            foreach (var asset in unusedAssets)
            {
                visitLookup.TryGetValue(asset.VisitId ?? Guid.Empty, out var visit);

                ws.Cell(row, 1).Value = index++;
                ws.Cell(row, 2).Value = visit?.VisitNumber;
                ws.Cell(row, 3).Value = visit?.SiteCode;
                ws.Cell(row, 4).Value = asset.AssetName;
                ws.Cell(row, 5).Value = asset.Quantity;
                ws.Cell(row, 6).Value = asset.Notes;
                ws.Cell(row, 7).Value = asset.RecordedAtUtc;
                row++;
            }
        }
        else
        {
            foreach (var visit in visits)
            {
                foreach (var material in visit.MaterialsUsed.Where(m => m.Quantity.Value > 0))
                {
                    ws.Cell(row, 1).Value = index++;
                    ws.Cell(row, 2).Value = visit.VisitNumber;
                    ws.Cell(row, 3).Value = visit.SiteCode;
                    ws.Cell(row, 4).Value = material.MaterialName;
                    ws.Cell(row, 5).Value = material.Quantity.Value;
                    ws.Cell(row, 7).Value = visit.ScheduledDate;
                    row++;
                }
            }
        }

        ws.Columns().AdjustToContents();
    }

    private static void BuildAlarmsCaptureSheet(XLWorkbook workbook, IReadOnlyList<Visit> visits)
    {
        var ws = workbook.AddWorksheet("alarms capture");
        ws.Cell(1, 1).Value = "Visit Number";
        ws.Cell(1, 2).Value = "Site Code";
        ws.Cell(1, 3).Value = "BTS capture ( Alarms )";
        ws.Cell(1, 4).Value = "3G capture";
        ws.Cell(1, 5).Value = "MW capture";

        var row = 2;
        foreach (var visit in visits)
        {
            ws.Cell(row, 1).Value = visit.VisitNumber;
            ws.Cell(row, 2).Value = visit.SiteCode;
            ws.Cell(row, 3).Value = visit.Photos.Any(p => p.Category == PhotoCategory.BTS);
            ws.Cell(row, 4).Value = visit.Photos.Any(p => p.Category == PhotoCategory.NodeB);
            ws.Cell(row, 5).Value = visit.Photos.Any(p => p.Category == PhotoCategory.MW);
            row++;
        }

        ws.Columns().AdjustToContents();
    }

    private static void BuildAuditMatrixSheet(XLWorkbook workbook, IReadOnlyList<ChecklistTemplate> templates)
    {
        var ws = workbook.AddWorksheet("Audit matrix SQI");
        ws.Cell(1, 1).Value = "File";
        ws.Cell(1, 2).Value = "Network Audit Checklist (applicable on entire radio sites)";

        var row = 2;
        foreach (var template in templates.Where(t => t.VisitType == VisitType.Audit).OrderByDescending(t => t.EffectiveFromUtc))
        {
            ws.Cell(row, 1).Value = "File Version";
            ws.Cell(row, 2).Value = template.Version;
            row++;

            foreach (var item in template.Items.OrderBy(i => i.OrderIndex))
            {
                ws.Cell(row, 1).Value = item.Category;
                ws.Cell(row, 2).Value = item.ItemName;
                row++;
            }
        }

        ws.Columns().AdjustToContents();
    }
}
