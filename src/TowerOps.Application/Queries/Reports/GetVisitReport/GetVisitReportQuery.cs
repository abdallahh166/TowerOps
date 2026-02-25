namespace TowerOps.Application.Queries.Reports.GetVisitReport;

using System;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Reports;

public record GetVisitReportQuery : IQuery<VisitReportDto>
{
    public Guid VisitId { get; init; }
}