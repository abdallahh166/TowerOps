namespace TelecomPM.Application.DTOs.Reports;

using System;
using System.Collections.Generic;
using TelecomPM.Domain.Enums;

public record MaterialUsageSummaryDto
{
    public Guid MaterialId { get; init; }
    public string MaterialCode { get; init; } = string.Empty;
    public string MaterialName { get; init; } = string.Empty;
    public MaterialCategory Category { get; init; }
    public Guid OfficeId { get; init; }
    public string OfficeName { get; init; } = string.Empty;
    
    // Stock Information
    public decimal CurrentStock { get; init; }
    public string Unit { get; init; } = string.Empty;
    public decimal MinimumStock { get; init; }
    public decimal ReorderQuantity { get; init; }
    public bool IsLowStock { get; init; }
    public decimal StockValue { get; init; } // CurrentStock * UnitCost
    
    // Usage Statistics
    public decimal TotalConsumed { get; init; }
    public decimal TotalPurchased { get; init; }
    public decimal TotalTransferred { get; init; }
    public int TransactionCount { get; init; }
    public int VisitUsageCount { get; init; }
    
    // Cost Information
    public decimal UnitCost { get; init; }
    public string Currency { get; init; } = string.Empty;
    public decimal TotalCost { get; init; } // TotalConsumed * UnitCost
    public decimal AverageCostPerVisit { get; init; }
    
    // Supplier Information
    public string? Supplier { get; init; }
    public DateTime? LastRestockDate { get; init; }
    public int DaysSinceLastRestock { get; init; }
    
    // Usage Trends
    public List<MaterialUsageTrendDto> UsageTrends { get; init; } = new();
    
    // Top Usage Sites
    public List<MaterialUsageBySiteDto> TopUsageSites { get; init; } = new();
    
    // Time Range
    public DateTime FromDate { get; init; }
    public DateTime ToDate { get; init; }
}

public record MaterialUsageTrendDto
{
    public DateTime Period { get; init; }
    public decimal Consumed { get; init; }
    public decimal Purchased { get; init; }
    public decimal AveragePerVisit { get; init; }
}

public record MaterialUsageBySiteDto
{
    public Guid SiteId { get; init; }
    public string SiteCode { get; init; } = string.Empty;
    public string SiteName { get; init; } = string.Empty;
    public decimal QuantityUsed { get; init; }
    public decimal TotalCost { get; init; }
    public int VisitCount { get; init; }
}

