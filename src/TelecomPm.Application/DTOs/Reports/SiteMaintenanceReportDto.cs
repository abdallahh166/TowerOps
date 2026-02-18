namespace TelecomPM.Application.DTOs.Reports;

using System;
using System.Collections.Generic;
using TelecomPM.Domain.Enums;

public record SiteMaintenanceReportDto
{
    public Guid SiteId { get; init; }
    public string SiteCode { get; init; } = string.Empty;
    public string SiteName { get; init; } = string.Empty;
    public string Region { get; init; } = string.Empty;
    public Guid OfficeId { get; init; }
    public string OfficeName { get; init; } = string.Empty;
    
    // Status Information
    public SiteStatus Status { get; init; }
    public SiteType SiteType { get; init; }
    public SiteComplexity Complexity { get; init; }
    
    // Assignment
    public Guid? AssignedEngineerId { get; init; }
    public string? AssignedEngineerName { get; init; }
    
    // Visit Metrics
    public int TotalVisits { get; init; }
    public int CompletedVisits { get; init; }
    public DateTime? LastVisitDate { get; init; }
    public DateTime? NextScheduledVisit { get; init; }
    public int DaysSinceLastVisit { get; init; }
    
    // Issues
    public int TotalIssues { get; init; }
    public int OpenIssues { get; init; }
    public int CriticalIssues { get; init; }
    public int ResolvedIssues { get; init; }
    
    // Maintenance History
    public List<SiteMaintenanceHistoryDto> MaintenanceHistory { get; init; } = new();
    
    // Material Usage
    public decimal TotalMaterialCost { get; init; }
    public int MaterialsUsedCount { get; init; }
    
    // Time Range
    public DateTime FromDate { get; init; }
    public DateTime ToDate { get; init; }
}

public record SiteMaintenanceHistoryDto
{
    public DateTime VisitDate { get; init; }
    public string VisitNumber { get; init; } = string.Empty;
    public VisitStatus VisitStatus { get; init; }
    public string EngineerName { get; init; } = string.Empty;
    public int IssuesFound { get; init; }
    public int IssuesResolved { get; init; }
    public decimal MaterialCost { get; init; }
    public string? Notes { get; init; }
}

