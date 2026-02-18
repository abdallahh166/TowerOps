namespace TelecomPM.Infrastructure.Services;

using ClosedXML.Excel;
using Microsoft.Extensions.Logging;
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Application.DTOs.Materials;
using TelecomPM.Domain.Interfaces.Repositories;

public class ExcelExportService : IExcelExportService
{
    private readonly IVisitRepository _visitRepository;
    private readonly ISiteRepository _siteRepository;
    private readonly ILogger<ExcelExportService> _logger;

    public ExcelExportService(
        IVisitRepository visitRepository,
        ISiteRepository siteRepository,
        ILogger<ExcelExportService> logger)
    {
        _visitRepository = visitRepository;
        _siteRepository = siteRepository;
        _logger = logger;
    }

    public async Task<byte[]> ExportVisitToExcelAsync(
        Guid visitId,
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

            using var workbook = new XLWorkbook();

            // Sheet 1: Visit Info
            CreateVisitInfoSheet(workbook, visit, site);

            // Sheet 2: Readings
            CreateReadingsSheet(workbook, visit);

            // Sheet 3: Photos Log
            CreatePhotosSheet(workbook, visit);

            // Sheet 4: Materials Used
            CreateMaterialsSheet(workbook, visit);

            // Sheet 5: Issues Found
            CreateIssuesSheet(workbook, visit);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to export visit {VisitId} to Excel", visitId);
            throw;
        }
    }

    private void CreateVisitInfoSheet(IXLWorkbook workbook, dynamic visit, dynamic site)
    {
        var worksheet = workbook.Worksheets.Add("Visit Info");

        // Header
        worksheet.Cell("A1").Value = "TelecomPM - Visit Report";
        worksheet.Cell("A1").Style.Font.Bold = true;
        worksheet.Cell("A1").Style.Font.FontSize = 16;

        // Visit Details
        var row = 3;
        worksheet.Cell($"A{row}").Value = "Visit Number:";
        worksheet.Cell($"B{row}").Value = visit.VisitNumber;

        row++;
        worksheet.Cell($"A{row}").Value = "Site Code:";
        worksheet.Cell($"B{row}").Value = visit.SiteCode;

        row++;
        worksheet.Cell($"A{row}").Value = "Site Name:";
        worksheet.Cell($"B{row}").Value = visit.SiteName;

        row++;
        worksheet.Cell($"A{row}").Value = "Engineer:";
        worksheet.Cell($"B{row}").Value = visit.EngineerName;

        row++;
        worksheet.Cell($"A{row}").Value = "Scheduled Date:";
        worksheet.Cell($"B{row}").Value = visit.ScheduledDate;

        row++;
        worksheet.Cell($"A{row}").Value = "Actual Start:";
        worksheet.Cell($"B{row}").Value = visit.ActualStartTime;

        row++;
        worksheet.Cell($"A{row}").Value = "Actual End:";
        worksheet.Cell($"B{row}").Value = visit.ActualEndTime;

        row++;
        worksheet.Cell($"A{row}").Value = "Status:";
        worksheet.Cell($"B{row}").Value = visit.Status.ToString();

        row++;
        worksheet.Cell($"A{row}").Value = "Completion:";
        worksheet.Cell($"B{row}").Value = $"{visit.CompletionPercentage}%";

        // Auto-fit columns
        worksheet.Columns().AdjustToContents();
    }

    private void CreateReadingsSheet(IXLWorkbook workbook, dynamic visit)
    {
        var worksheet = workbook.Worksheets.Add("Readings");

        // Headers
        worksheet.Cell("A1").Value = "Reading Type";
        worksheet.Cell("B1").Value = "Category";
        worksheet.Cell("C1").Value = "Value";
        worksheet.Cell("D1").Value = "Unit";
        worksheet.Cell("E1").Value = "Min Acceptable";
        worksheet.Cell("F1").Value = "Max Acceptable";
        worksheet.Cell("G1").Value = "Within Range";
        worksheet.Cell("H1").Value = "Phase";
        worksheet.Cell("I1").Value = "Equipment";
        worksheet.Cell("J1").Value = "Notes";
        worksheet.Cell("K1").Value = "Measured At";

        // Style headers
        var headerRange = worksheet.Range("A1:K1");
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

        // Data
        var row = 2;
        foreach (var reading in visit.Readings)
        {
            worksheet.Cell($"A{row}").Value = reading.ReadingType;
            worksheet.Cell($"B{row}").Value = reading.Category;
            worksheet.Cell($"C{row}").Value = reading.Value;
            worksheet.Cell($"D{row}").Value = reading.Unit;
            worksheet.Cell($"E{row}").Value = reading.MinAcceptable;
            worksheet.Cell($"F{row}").Value = reading.MaxAcceptable;
            worksheet.Cell($"G{row}").Value = reading.IsWithinRange ? "Yes" : "No";
            worksheet.Cell($"H{row}").Value = reading.Phase;
            worksheet.Cell($"I{row}").Value = reading.Equipment;
            worksheet.Cell($"J{row}").Value = reading.Notes;
            worksheet.Cell($"K{row}").Value = reading.MeasuredAt;

            // Highlight out of range
            if (!reading.IsWithinRange)
            {
                worksheet.Range($"A{row}:K{row}").Style.Fill.BackgroundColor = XLColor.LightPink;
            }

            row++;
        }

        worksheet.Columns().AdjustToContents();
    }

    private void CreatePhotosSheet(IXLWorkbook workbook, dynamic visit)
    {
        var worksheet = workbook.Worksheets.Add("Photos");

        // Headers
        worksheet.Cell("A1").Value = "Type";
        worksheet.Cell("B1").Value = "Category";
        worksheet.Cell("C1").Value = "Item Name";
        worksheet.Cell("D1").Value = "Description";
        worksheet.Cell("E1").Value = "File Path";
        worksheet.Cell("F1").Value = "Captured At";

        var headerRange = worksheet.Range("A1:F1");
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

        var row = 2;
        foreach (var photo in visit.Photos)
        {
            worksheet.Cell($"A{row}").Value = photo.Type.ToString();
            worksheet.Cell($"B{row}").Value = photo.Category.ToString();
            worksheet.Cell($"C{row}").Value = photo.ItemName;
            worksheet.Cell($"D{row}").Value = photo.Description;
            worksheet.Cell($"E{row}").Value = photo.FilePath;
            worksheet.Cell($"E{row}").SetHyperlink(new XLHyperlink(photo.FilePath));
            worksheet.Cell($"F{row}").Value = photo.CapturedAt;

            row++;
        }

        worksheet.Columns().AdjustToContents();
    }

    private void CreateMaterialsSheet(IXLWorkbook workbook, dynamic visit)
    {
        var worksheet = workbook.Worksheets.Add("Materials");

        // Headers
        worksheet.Cell("A1").Value = "Material Code";
        worksheet.Cell("B1").Value = "Material Name";
        worksheet.Cell("C1").Value = "Category";
        worksheet.Cell("D1").Value = "Quantity";
        worksheet.Cell("E1").Value = "Unit";
        worksheet.Cell("F1").Value = "Unit Cost";
        worksheet.Cell("G1").Value = "Total Cost";
        worksheet.Cell("H1").Value = "Reason";
        worksheet.Cell("I1").Value = "Status";
        worksheet.Cell("J1").Value = "Used At";

        var headerRange = worksheet.Range("A1:J1");
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

        var row = 2;
        decimal totalCost = 0;

        foreach (var material in visit.MaterialsUsed)
        {
            worksheet.Cell($"A{row}").Value = material.MaterialCode;
            worksheet.Cell($"B{row}").Value = material.MaterialName;
            worksheet.Cell($"C{row}").Value = material.Category.ToString();
            worksheet.Cell($"D{row}").Value = material.Quantity.Value;
            worksheet.Cell($"E{row}").Value = material.Quantity.Unit.ToString();
            worksheet.Cell($"F{row}").Value = material.UnitCost.Amount;
            worksheet.Cell($"G{row}").Value = material.TotalCost.Amount;
            worksheet.Cell($"H{row}").Value = material.Reason;
            worksheet.Cell($"I{row}").Value = material.Status.ToString();
            worksheet.Cell($"J{row}").Value = material.UsedAt;

            totalCost += material.TotalCost.Amount;
            row++;
        }

        // Total row
        if (visit.MaterialsUsed.Count > 0)
        {
            worksheet.Cell($"F{row}").Value = "TOTAL:";
            worksheet.Cell($"F{row}").Style.Font.Bold = true;
            worksheet.Cell($"G{row}").Value = totalCost;
            worksheet.Cell($"G{row}").Style.Font.Bold = true;
            worksheet.Cell($"G{row}").Style.Fill.BackgroundColor = XLColor.LightYellow;
        }

        worksheet.Columns().AdjustToContents();
    }

    private void CreateIssuesSheet(IXLWorkbook workbook, dynamic visit)
    {
        var worksheet = workbook.Worksheets.Add("Issues");

        // Headers
        worksheet.Cell("A1").Value = "Category";
        worksheet.Cell("B1").Value = "Severity";
        worksheet.Cell("C1").Value = "Title";
        worksheet.Cell("D1").Value = "Description";
        worksheet.Cell("E1").Value = "Status";
        worksheet.Cell("F1").Value = "Reported At";
        worksheet.Cell("G1").Value = "Resolved At";
        worksheet.Cell("H1").Value = "Resolution";

        var headerRange = worksheet.Range("A1:H1");
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

        var row = 2;
        foreach (var issue in visit.IssuesFound)
        {
            worksheet.Cell($"A{row}").Value = issue.Category.ToString();
            worksheet.Cell($"B{row}").Value = issue.Severity.ToString();
            worksheet.Cell($"C{row}").Value = issue.Title;
            worksheet.Cell($"D{row}").Value = issue.Description;
            worksheet.Cell($"E{row}").Value = issue.Status.ToString();
            worksheet.Cell($"F{row}").Value = issue.ReportedAt;
            worksheet.Cell($"G{row}").Value = issue.ResolvedAt;
            worksheet.Cell($"H{row}").Value = issue.Resolution;

            // Highlight critical issues
            if (issue.Severity.ToString() == "Critical")
            {
                worksheet.Range($"A{row}:H{row}").Style.Fill.BackgroundColor = XLColor.LightPink;
            }

            row++;
        }

        worksheet.Columns().AdjustToContents();
    }

    public async Task<byte[]> ExportMaterialsToExcelAsync(
        List<MaterialDto> materials,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Materials");

            // Headers
            worksheet.Cell("A1").Value = "Code";
            worksheet.Cell("B1").Value = "Name";
            worksheet.Cell("C1").Value = "Description";
            worksheet.Cell("D1").Value = "Category";
            worksheet.Cell("E1").Value = "Current Stock";
            worksheet.Cell("F1").Value = "Unit";
            worksheet.Cell("G1").Value = "Minimum Stock";
            worksheet.Cell("H1").Value = "Unit Cost";
            worksheet.Cell("I1").Value = "Currency";
            worksheet.Cell("J1").Value = "Status";

            var headerRange = worksheet.Range("A1:J1");
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

            var row = 2;
            foreach (var material in materials)
            {
                worksheet.Cell($"A{row}").Value = material.Code;
                worksheet.Cell($"B{row}").Value = material.Name;
                worksheet.Cell($"C{row}").Value = material.Description;
                worksheet.Cell($"D{row}").Value = material.Category.ToString();
                worksheet.Cell($"E{row}").Value = material.CurrentStock;
                worksheet.Cell($"F{row}").Value = material.Unit;
                worksheet.Cell($"G{row}").Value = material.MinimumStock;
                worksheet.Cell($"H{row}").Value = material.UnitCost;
                worksheet.Cell($"I{row}").Value = material.Currency;
                worksheet.Cell($"J{row}").Value = material.IsActive ? "Active" : "Inactive";

                // Highlight low stock
                if (material.IsLowStock)
                {
                    worksheet.Range($"A{row}:J{row}").Style.Fill.BackgroundColor = XLColor.LightYellow;
                }

                row++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to export materials to Excel");
            throw;
        }
    }
}