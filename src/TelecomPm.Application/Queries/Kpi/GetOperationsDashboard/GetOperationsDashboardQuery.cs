namespace TelecomPM.Application.Queries.Kpi.GetOperationsDashboard;

using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Kpi;
using TelecomPM.Domain.Enums;

public record GetOperationsDashboardQuery : IQuery<OperationsKpiDashboardDto>
{
    public DateTime? FromDateUtc { get; init; }
    public DateTime? ToDateUtc { get; init; }
    public string? OfficeCode { get; init; }
    public SlaClass? SlaClass { get; init; }
}
