namespace TelecomPM.Application.DTOs.Reports;

using System;
using System.Collections.Generic;
using TelecomPM.Domain.Enums;

public record EngineerPerformanceReportDto
{
    public Guid EngineerId { get; init; }
    public string EngineerName { get; init; } = string.Empty;
    public string EngineerEmail { get; init; } = string.Empty;
    public Guid OfficeId { get; init; }
    public string OfficeName { get; init; } = string.Empty;
    public List<string> Specializations { get; init; } = new();
    
    // Assignment Metrics
    public int AssignedSitesCount { get; init; }
    public int? MaxAssignedSites { get; init; }
    public int CapacityUtilization { get; init; } // Percentage
    
    // Visit Metrics
    public int TotalVisits { get; init; }
    public int CompletedVisits { get; init; }
    public int ApprovedVisits { get; init; }
    public int RejectedVisits { get; init; }
    public int PendingReviewVisits { get; init; }
    public int OnTimeVisits { get; init; }
    public int OverdueVisits { get; init; }
    
    // Performance Rates
    public decimal CompletionRate { get; init; } // Percentage
    public decimal ApprovalRate { get; init; } // Percentage
    public decimal OnTimeRate { get; init; } // Percentage
    public decimal? PerformanceRating { get; init; }
    
    // Quality Metrics
    public int VisitsNeedingCorrection { get; init; }
    public int CriticalIssuesReported { get; init; }
    public decimal AverageVisitDuration { get; init; } // Hours
    
    // Time Range
    public DateTime FromDate { get; init; }
    public DateTime ToDate { get; init; }
    
    // Comparison with Previous Period
    public decimal? CompletionRateChange { get; init; } // Percentage points
    public decimal? ApprovalRateChange { get; init; } // Percentage points
}

