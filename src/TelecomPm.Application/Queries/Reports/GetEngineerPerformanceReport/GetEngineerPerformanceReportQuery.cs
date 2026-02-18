namespace TelecomPM.Application.Queries.Reports.GetEngineerPerformanceReport;

using System;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Reports;

public record GetEngineerPerformanceReportQuery : IQuery<EngineerPerformanceReportDto>
{
    public Guid EngineerId { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
}

