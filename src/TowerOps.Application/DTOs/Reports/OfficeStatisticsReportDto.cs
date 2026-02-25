namespace TowerOps.Application.DTOs.Reports;

using System;
using System.Collections.Generic;

public record OfficeStatisticsReportDto
{
    public Guid OfficeId { get; init; }
    public string OfficeCode { get; init; } = string.Empty;
    public string OfficeName { get; init; } = string.Empty;
    public string Region { get; init; } = string.Empty;
    
    // Site Statistics
    public int TotalSites { get; init; }
    public int ActiveSites { get; init; }
    public int InactiveSites { get; init; }
    public int AssignedSites { get; init; }
    public int UnassignedSites { get; init; }
    public int SitesNeedingMaintenance { get; init; }
    
    // Personnel Statistics
    public int TotalEngineers { get; init; }
    public int ActiveEngineers { get; init; }
    public int TotalTechnicians { get; init; }
    public int ActiveTechnicians { get; init; }
    public int TotalUsers { get; init; }
    
    // Visit Statistics
    public int TotalVisits { get; init; }
    public int ScheduledVisits { get; init; }
    public int CompletedVisits { get; init; }
    public int ApprovedVisits { get; init; }
    public int RejectedVisits { get; init; }
    public int PendingReviewVisits { get; init; }
    public int OverdueVisits { get; init; }
    
    // Material Statistics
    public int TotalMaterials { get; init; }
    public int ActiveMaterials { get; init; }
    public int LowStockMaterials { get; init; }
    public decimal TotalMaterialValue { get; init; }
    
    // Performance Metrics
    public decimal AverageVisitCompletionRate { get; init; } // Percentage
    public decimal AverageVisitApprovalRate { get; init; } // Percentage
    public decimal AverageEngineerPerformance { get; init; } // Rating
    
    // Issue Statistics
    public int TotalIssues { get; init; }
    public int OpenIssues { get; init; }
    public int CriticalIssues { get; init; }
    public int ResolvedIssues { get; init; }
    
    // Engineer Performance Summary
    public List<EngineerSummaryDto> TopEngineers { get; init; } = new();
    
    // Time Range
    public DateTime FromDate { get; init; }
    public DateTime ToDate { get; init; }
}

public record EngineerSummaryDto
{
    public Guid EngineerId { get; init; }
    public string EngineerName { get; init; } = string.Empty;
    public int AssignedSites { get; init; }
    public int CompletedVisits { get; init; }
    public decimal CompletionRate { get; init; }
    public decimal? PerformanceRating { get; init; }
}

