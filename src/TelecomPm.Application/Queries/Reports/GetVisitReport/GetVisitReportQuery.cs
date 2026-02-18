namespace TelecomPM.Application.Queries.Reports.GetVisitReport;

using System;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Reports;

public record GetVisitReportQuery : IQuery<VisitReportDto>
{
    public Guid VisitId { get; init; }
}