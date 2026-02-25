namespace TowerOps.Application.Queries.Materials.GetMaterialUsageReport;

using System;
using System.Collections.Generic;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Reports;

public record GetMaterialUsageReportQuery : IQuery<MaterialUsageReportDto>
{
    public Guid? MaterialId { get; init; }
    public Guid? OfficeId { get; init; }
    public DateTime FromDate { get; init; }
    public DateTime ToDate { get; init; }
}

