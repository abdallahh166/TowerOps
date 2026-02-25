namespace TowerOps.Application.Queries.Reports.GetMaterialUsageSummary;

using System;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Reports;

public record GetMaterialUsageSummaryQuery : IQuery<MaterialUsageSummaryDto>
{
    public Guid MaterialId { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
}

