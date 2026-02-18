namespace TelecomPM.Application.Queries.Reports.GetVisitCompletionTrends;

using System;
using System.Collections.Generic;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Reports;

public record GetVisitCompletionTrendsQuery : IQuery<List<VisitCompletionTrendDto>>
{
    public Guid? OfficeId { get; init; }
    public Guid? EngineerId { get; init; }
    public DateTime FromDate { get; init; }
    public DateTime ToDate { get; init; }
    public TrendPeriod Period { get; init; } = TrendPeriod.Monthly;
}

public record VisitCompletionTrendDto
{
    public DateTime Period { get; init; }
    public int TotalVisits { get; init; }
    public int CompletedVisits { get; init; }
    public int ApprovedVisits { get; init; }
    public int RejectedVisits { get; init; }
    public decimal CompletionRate { get; init; }
    public decimal ApprovalRate { get; init; }
    public int OverdueVisits { get; init; }
}

public enum TrendPeriod
{
    Daily,
    Weekly,
    Monthly,
    Yearly
}

