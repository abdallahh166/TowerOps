namespace TowerOps.Api.Mappings;

using TowerOps.Application.Queries.Kpi.GetOperationsDashboard;
using TowerOps.Domain.Enums;

public static class KpiContractMapper
{
    public static GetOperationsDashboardQuery ToOperationsDashboardQuery(
        DateTime? fromDateUtc,
        DateTime? toDateUtc,
        string? officeCode,
        SlaClass? slaClass)
        => new()
        {
            FromDateUtc = fromDateUtc,
            ToDateUtc = toDateUtc,
            OfficeCode = officeCode,
            SlaClass = slaClass
        };
}
