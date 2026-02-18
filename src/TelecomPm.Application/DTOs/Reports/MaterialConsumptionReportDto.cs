using System;
using System.Collections.Generic;

namespace TelecomPM.Application.DTOs.Reports;

public record MaterialConsumptionReportDto
{
    public Guid OfficeId { get; init; }
    public string OfficeName { get; init; } = string.Empty;
    public DateTime FromDate { get; init; }
    public DateTime ToDate { get; init; }
    public List<MaterialConsumptionItemDto> Items { get; init; } = new();
    public decimal TotalCost { get; init; }
}

public record MaterialConsumptionItemDto
{
    public string MaterialCode { get; init; } = string.Empty;
    public string MaterialName { get; init; } = string.Empty;
    public decimal TotalQuantity { get; init; }
    public string Unit { get; init; } = string.Empty;
    public decimal TotalCost { get; init; }
    public int VisitsCount { get; init; }
}