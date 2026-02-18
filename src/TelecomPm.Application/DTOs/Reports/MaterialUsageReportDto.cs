namespace TelecomPM.Application.DTOs.Reports;

using System;
using System.Collections.Generic;

public record MaterialUsageReportDto
{
    public Guid? OfficeId { get; init; }
    public DateTime FromDate { get; init; }
    public DateTime ToDate { get; init; }
    public List<MaterialUsageItemDto> Items { get; init; } = new();
    public int TotalMaterialsTracked { get; init; }
    public int TotalTransactions { get; init; }
}

public record MaterialUsageItemDto
{
    public Guid MaterialId { get; init; }
    public string MaterialCode { get; init; } = string.Empty;
    public string MaterialName { get; init; } = string.Empty;
    public decimal TotalUsed { get; init; }
    public decimal TotalPurchased { get; init; }
    public string Unit { get; init; } = string.Empty;
    public decimal TotalCost { get; init; }
}

