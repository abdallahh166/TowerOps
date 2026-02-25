namespace TowerOps.Application.Queries.Reports.GetOfficeStatisticsReport;

using System;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Reports;

public record GetOfficeStatisticsReportQuery : IQuery<OfficeStatisticsReportDto>
{
    public Guid OfficeId { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
}

