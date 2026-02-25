using TowerOps.Domain.Entities.WorkOrders;

namespace TowerOps.Domain.Specifications.WorkOrderSpecifications;

public sealed class ContractorScorecardWorkOrdersSpecification : BaseSpecification<WorkOrder>
{
    public ContractorScorecardWorkOrdersSpecification(string officeCode, DateTime fromDateUtc, DateTime toDateUtc)
        : base(wo =>
            wo.OfficeCode == officeCode &&
            wo.CreatedAt >= fromDateUtc &&
            wo.CreatedAt <= toDateUtc)
    {
        AddOrderBy(wo => wo.CreatedAt);
    }
}
