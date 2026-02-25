namespace TowerOps.Application.Queries.Reports.GetIssueAnalyticsReport;

using System;
using System.Collections.Generic;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Reports;

public record GetIssueAnalyticsReportQuery : IQuery<IssueAnalyticsReportDto>
{
    public Guid? OfficeId { get; init; }
    public Guid? SiteId { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
}

public record IssueAnalyticsReportDto
{
    public int TotalIssues { get; init; }
    public int OpenIssues { get; init; }
    public int ResolvedIssues { get; init; }
    public int CriticalIssues { get; init; }
    public int HighIssues { get; init; }
    public int MediumIssues { get; init; }
    public int LowIssues { get; init; }
    public decimal ResolutionRate { get; init; }
    public decimal AverageResolutionTime { get; init; } // Days
    public List<IssueByCategoryDto> IssuesByCategory { get; init; } = new();
    public List<IssueBySeverityDto> IssuesBySeverity { get; init; } = new();
    public List<IssueBySiteDto> TopSitesWithIssues { get; init; } = new();
    public DateTime FromDate { get; init; }
    public DateTime ToDate { get; init; }
}

public record IssueByCategoryDto
{
    public string Category { get; init; } = string.Empty;
    public int TotalCount { get; init; }
    public int OpenCount { get; init; }
    public int ResolvedCount { get; init; }
}

public record IssueBySeverityDto
{
    public string Severity { get; init; } = string.Empty;
    public int TotalCount { get; init; }
    public int OpenCount { get; init; }
    public int ResolvedCount { get; init; }
}

public record IssueBySiteDto
{
    public Guid SiteId { get; init; }
    public string SiteCode { get; init; } = string.Empty;
    public string SiteName { get; init; } = string.Empty;
    public int TotalIssues { get; init; }
    public int OpenIssues { get; init; }
    public int CriticalIssues { get; init; }
}

