namespace TelecomPM.Application.Queries.Reports.GetMaterialUsageSummary;

using System;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Reports;

public record GetMaterialUsageSummaryQuery : IQuery<MaterialUsageSummaryDto>
{
    public Guid MaterialId { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
}

