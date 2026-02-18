namespace TelecomPM.Infrastructure.Services;

using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Domain.Entities.Sites;
using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Interfaces.Repositories;

public class ReportGenerationService : IReportGenerationService
{
    private readonly IVisitRepository _visitRepository;
    private readonly ISiteRepository _siteRepository;
    private readonly ILogger<ReportGenerationService> _logger;

    static ReportGenerationService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public ReportGenerationService(
        IVisitRepository visitRepository,
        ISiteRepository siteRepository,
        ILogger<ReportGenerationService> logger)
    {
        _visitRepository = visitRepository;
        _siteRepository = siteRepository;
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
                ReportFormat.Excel => await GenerateExcelReport(visit, site),
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

                // Header
                page.Header().Element(c => CreateHeader(c, visit, site));

                // Content
                page.Content().Element(c => CreateContent(c, visit, site));

                // Footer
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

    private void CreateContent(IContainer container, Visit visit, Site site)
    {
        container.PaddingVertical(10).Column(column =>
        {
            // Site Information
            column.Item().Text("Site Information").FontSize(14).Bold();
            column.Item().PaddingBottom(5);
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(3);
                });

                table.Cell().Border(1).Padding(5).Text("Region");
                table.Cell().Border(1).Padding(5).Text(site.Region ?? "N/A");

                table.Cell().Border(1).Padding(5).Text("Complexity");
                table.Cell().Border(1).Padding(5).Text(site.Complexity.ToString());

                table.Cell().Border(1).Padding(5).Text("Status");
                table.Cell().Border(1).Padding(5).Text(site.Status.ToString());
            });

            column.Item().PaddingTop(10);

            // Visit Timeline
            column.Item().Text("Visit Timeline").FontSize(14).Bold();
            column.Item().PaddingBottom(5);
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(3);
                });

                table.Cell().Border(1).Padding(5).Text("Scheduled");
                table.Cell().Border(1).Padding(5).Text(visit.ScheduledDate.ToString("dd/MM/yyyy HH:mm"));

                table.Cell().Border(1).Padding(5).Text("Start Time");
                table.Cell().Border(1).Padding(5).Text(visit.ActualStartTime?.ToString("dd/MM/yyyy HH:mm") ?? "N/A");

                table.Cell().Border(1).Padding(5).Text("End Time");
                table.Cell().Border(1).Padding(5).Text(visit.ActualEndTime?.ToString("dd/MM/yyyy HH:mm") ?? "N/A");

                table.Cell().Border(1).Padding(5).Text("Duration");
                table.Cell().Border(1).Padding(5).Text(visit.ActualDuration?.ToString() ?? "N/A");
            });

            column.Item().PaddingTop(10);

            // Summary Statistics
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

            // Engineer Notes
            if (!string.IsNullOrWhiteSpace(visit.EngineerNotes))
            {
                column.Item().PaddingTop(10);
                column.Item().Text("Engineer Notes").FontSize(14).Bold();
                column.Item().PaddingBottom(5);
                column.Item().Border(1).Padding(10).Text(visit.EngineerNotes);
            }
        });
    }

    private async Task<byte[]> GenerateExcelReport(Visit visit, Site site)
    {
        // Reuse ExcelExportService logic
        await Task.CompletedTask;
        throw new NotImplementedException("Use ExcelExportService instead");
    }

    public Task<byte[]> GenerateMaterialConsumptionReportAsync(
        Guid officeId,
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default)
    {
        // TODO: Implement material consumption report
        throw new NotImplementedException();
    }

    public Task<byte[]> GenerateEngineerPerformanceReportAsync(
        Guid engineerId,
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default)
    {
        // TODO: Implement engineer performance report
        throw new NotImplementedException();
    }
}