using TowerOps.Domain.Entities.Visits;

namespace TowerOps.Domain.Specifications.VisitSpecifications;

public sealed class ContractorScorecardVisitsSpecification : BaseSpecification<Visit>
{
    public ContractorScorecardVisitsSpecification(IReadOnlyCollection<string> siteCodes, DateTime fromDateUtc, DateTime toDateUtc)
        : base(v =>
            siteCodes.Contains(v.SiteCode) &&
            v.CreatedAt >= fromDateUtc &&
            v.CreatedAt <= toDateUtc)
    {
        AddInclude(v => v.Photos);
        AddInclude(v => v.Readings);
        AddInclude(v => v.Checklists);
        AddInclude(v => v.MaterialsUsed);
        AddOrderBy(v => v.CreatedAt);
    }
}
