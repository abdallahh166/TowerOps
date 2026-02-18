namespace TelecomPM.Application.Queries.Reports.GetOfficeStatisticsReport;

using System;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Reports;

public record GetOfficeStatisticsReportQuery : IQuery<OfficeStatisticsReportDto>
{
    public Guid OfficeId { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
}

