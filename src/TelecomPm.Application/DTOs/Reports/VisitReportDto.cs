namespace TelecomPM.Application.DTOs.Reports;

using System.Collections.Generic;
using TelecomPM.Application.DTOs.Sites;
using TelecomPM.Application.DTOs.Visits;

public record VisitReportDto
{
    public VisitDetailDto Visit { get; init; } = null!;
    public SiteDetailDto Site { get; init; } = null!;
    public List<PhotoComparisonDto> PhotoComparisons { get; init; } = new();
    public decimal TotalMaterialCost { get; init; }
}

public record PhotoComparisonDto
{
    public string ItemName { get; init; } = string.Empty;
    public string BeforePhotoUrl { get; init; } = string.Empty;
    public string AfterPhotoUrl { get; init; } = string.Empty;
    public string? BeforeDescription { get; init; }
    public string? AfterDescription { get; init; }
}