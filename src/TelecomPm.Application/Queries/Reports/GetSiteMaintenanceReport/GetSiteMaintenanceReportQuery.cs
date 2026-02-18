namespace TelecomPM.Application.Queries.Reports.GetSiteMaintenanceReport;

using System;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Reports;

public record GetSiteMaintenanceReportQuery : IQuery<SiteMaintenanceReportDto>
{
    public Guid SiteId { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
}

