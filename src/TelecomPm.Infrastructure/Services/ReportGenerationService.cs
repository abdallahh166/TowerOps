namespace TelecomPM.Infrastructure.Services;

using System.Globalization;
using System.Text;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Domain.Entities.Materials;
using TelecomPM.Domain.Entities.Sites;
using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Interfaces.Repositories;

public class ReportGenerationService : IReportGenerationService
{
    private readonly IVisitRepository _visitRepository;
    private readonly ISiteRepository _siteRepository;
    private readonly IMaterialRepository _materialRepository;
    private readonly IExcelExportService _excelExportService;
    private readonly ILogger<ReportGenerationService> _logger;

    static ReportGenerationService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public ReportGenerationService(
        IVisitRepository visitRepository,
        ISiteRepository siteRepository,
        IMaterialRepository materialRepository,
        IExcelExportService excelExportService,
        ILogger<ReportGenerationService> logger)
    {
        _visitRepository = visitRepository;
        _siteRepository = siteRepository;
        _materialRepository = materialRepository;
        _excelExportService = excelExportService;
        _logger = logger;
    }

    public async Task<byte[]> GenerateVisitReportAsync(
        Guid visitId,
        ReportFormat format,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var visit = await _visitRepository.GetByIdAsync(visitId, cancellationToken);
            if (visit == null)
                throw new InvalidOperationException($"Visit {visitId} not found");

            var site = await _siteRepository.GetByIdAsync(visit.SiteId, cancellationToken);
            if (site == null)
                throw new InvalidOperationException($"Site {visit.SiteId} not found");

            return format switch
            {
                ReportFormat.PDF => GeneratePdfReport(visit, site),
                ReportFormat.Excel => await GenerateExcelReport(visit, cancellationToken),
                _ => throw new NotSupportedException($"Format {format} is not supported")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate visit report for {VisitId}", visitId);
            throw;
        }
    }

    private byte[] GeneratePdfReport(Visit visit, Site site)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Element(c => CreateHeader(c, visit, site));
                page.Content().Element(c => CreateContent(c, visit));
                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Page ");
                    x.CurrentPageNumber();
                    x.Span(" of ");
                    x.TotalPages();
                });
            });
        });

        return document.GeneratePdf();
    }

    private void CreateHeader(IContainer container, Visit visit, Site site)
    {
        container.Column(column =>
        {
            column.Item().Text("TelecomPM - Visit Report")
                .FontSize(20)
                .Bold()
                .FontColor(Colors.Blue.Medium);

            column.Item().PaddingVertical(5);

            column.Item().Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text($"Visit Number: {visit.VisitNumber}").Bold();
                    col.Item().Text($"Site: {site.SiteCode} - {site.Name}");
                    col.Item().Text($"Engineer: {visit.EngineerName ?? "N/A"}");
                });

                row.RelativeItem().Column(col =>
                {
                    col.Item().AlignRight().Text($"Date: {visit.ScheduledDate:dd/MM/yyyy}");
                    col.Item().AlignRight().Text($"Status: {visit.Status}");
                    col.Item().AlignRight().Text($"Completion: {visit.CompletionPercentage}%");
                });
            });

            column.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Medium);
        });
    }

    private void CreateContent(IContainer container, Visit visit)
    {
        container.PaddingVertical(10).Column(column =>
        {
            column.Item().Text("Summary").FontSize(14).Bold();
            column.Item().PaddingBottom(5);
            column.Item().Row(row =>
            {
                row.RelativeItem().Border(1).Padding(10).Column(col =>
                {
                    col.Item().AlignCenter().Text("Photos").Bold();
                    col.Item().AlignCenter().Text(visit.Photos?.Count.ToString() ?? "0").FontSize(20).FontColor(Colors.Blue.Medium);
                });

                row.RelativeItem().Border(1).Padding(10).Column(col =>
                {
                    col.Item().AlignCenter().Text("Readings").Bold();
                    col.Item().AlignCenter().Text(visit.Readings?.Count.ToString() ?? "0").FontSize(20).FontColor(Colors.Green.Medium);
                });

                row.RelativeItem().Border(1).Padding(10).Column(col =>
                {
                    col.Item().AlignCenter().Text("Materials").Bold();
                    col.Item().AlignCenter().Text(visit.MaterialsUsed?.Count.ToString() ?? "0").FontSize(20).FontColor(Colors.Orange.Medium);
                });

                row.RelativeItem().Border(1).Padding(10).Column(col =>
                {
                    col.Item().AlignCenter().Text("Issues").Bold();
                    col.Item().AlignCenter().Text(visit.IssuesFound?.Count.ToString() ?? "0").FontSize(20).FontColor(Colors.Red.Medium);
                });
            });

            if (!string.IsNullOrWhiteSpace(visit.EngineerNotes))
            {
                column.Item().PaddingTop(10);
                column.Item().Text("Engineer Notes").FontSize(14).Bold();
                column.Item().PaddingBottom(5);
                column.Item().Border(1).Padding(10).Text(visit.EngineerNotes);
            }
        });
    }

    private Task<byte[]> GenerateExcelReport(Visit visit, CancellationToken cancellationToken)
    {
        return _excelExportService.ExportVisitToExcelAsync(visit.Id, cancellationToken);
    }

    public async Task<byte[]> GenerateMaterialConsumptionReportAsync(
        Guid officeId,
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default)
    {
        if (from > to)
        {
            throw new ArgumentException("from must be less than or equal to to");
        }

        var materials = await _materialRepository.GetByOfficeIdAsNoTrackingAsync(officeId, cancellationToken);
        var csv = BuildMaterialConsumptionCsv(materials, from, to);
        return Encoding.UTF8.GetBytes(csv);
    }

    public async Task<byte[]> GenerateEngineerPerformanceReportAsync(
        Guid engineerId,
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default)
    {
        if (from > to)
        {
            throw new ArgumentException("from must be less than or equal to to");
        }

        var visits = await _visitRepository.GetByEngineerIdAsNoTrackingAsync(engineerId, cancellationToken);
        var filtered = visits
            .Where(v => v.ScheduledDate >= from && v.ScheduledDate <= to)
            .ToList();

        var completed = filtered.Count(v => v.Status is VisitStatus.Completed or VisitStatus.Submitted or VisitStatus.Approved);
        var approved = filtered.Count(v => v.Status == VisitStatus.Approved);
        var avgCompletion = filtered.Count == 0 ? 0 : filtered.Average(v => v.CompletionPercentage);

        var csv = new StringBuilder();
        csv.AppendLine("EngineerId,FromUtc,ToUtc,VisitsScheduled,VisitsCompletedOrBeyond,VisitsApproved,AverageCompletionPercentage");
        csv.AppendLine(string.Join(",",
            engineerId,
            FormatUtc(from),
            FormatUtc(to),
            filtered.Count,
            completed,
            approved,
            avgCompletion.ToString("F2", CultureInfo.InvariantCulture)));

        return Encoding.UTF8.GetBytes(csv.ToString());
    }

    private static string BuildMaterialConsumptionCsv(IReadOnlyList<Material> materials, DateTime from, DateTime to)
    {
        var csv = new StringBuilder();
        csv.AppendLine("MaterialCode,MaterialName,TransactionDateUtc,TransactionType,Quantity,Unit,PerformedBy,Reason");

        foreach (var material in materials)
        {
            var consumptionTx = material.Transactions
                .Where(t => t.TransactionDate >= from && t.TransactionDate <= to)
                .Where(t => t.Type is TransactionType.Usage or TransactionType.Transfer)
                .OrderBy(t => t.TransactionDate);

            foreach (var tx in consumptionTx)
            {
                csv.AppendLine(string.Join(",",
                    Escape(material.Code),
                    Escape(material.Name),
                    FormatUtc(tx.TransactionDate),
                    tx.Type,
                    tx.Quantity.Value.ToString(CultureInfo.InvariantCulture),
                    tx.Quantity.Unit,
                    Escape(tx.PerformedBy),
                    Escape(tx.Reason)));
            }
        }

        return csv.ToString();
    }

    private static string FormatUtc(DateTime value)
    {
        var utc = value.Kind == DateTimeKind.Utc ? value : value.ToUniversalTime();
        return utc.ToString("O", CultureInfo.InvariantCulture);
    }

    private static string Escape(string? input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        if (!input.Contains(',') && !input.Contains('"') && !input.Contains('\n'))
        {
            return input;
        }

        return $"\"{input.Replace("\"", "\"\"")}\"";
    }
}
