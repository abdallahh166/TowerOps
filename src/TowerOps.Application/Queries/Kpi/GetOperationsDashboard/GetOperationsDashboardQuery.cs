namespace TowerOps.Application.Queries.Kpi.GetOperationsDashboard;

using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Kpi;
using TowerOps.Domain.Enums;

public record GetOperationsDashboardQuery : IQuery<OperationsKpiDashboardDto>
{
    public DateTime? FromDateUtc { get; init; }
    public DateTime? ToDateUtc { get; init; }
    public string? OfficeCode { get; init; }
    public SlaClass? SlaClass { get; init; }
}
