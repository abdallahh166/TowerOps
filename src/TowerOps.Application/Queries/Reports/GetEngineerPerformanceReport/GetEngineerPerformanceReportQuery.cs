namespace TowerOps.Application.Queries.Reports.GetEngineerPerformanceReport;

using System;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Reports;

public record GetEngineerPerformanceReportQuery : IQuery<EngineerPerformanceReportDto>
{
    public Guid EngineerId { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
}

