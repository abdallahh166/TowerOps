namespace TowerOps.Application.Queries.Reports.GetSiteMaintenanceReport;

using System;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Reports;

public record GetSiteMaintenanceReportQuery : IQuery<SiteMaintenanceReportDto>
{
    public Guid SiteId { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
}

