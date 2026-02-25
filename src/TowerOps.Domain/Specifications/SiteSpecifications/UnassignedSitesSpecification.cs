using TowerOps.Domain.Entities.Sites;
using TowerOps.Domain.Enums;

namespace TowerOps.Domain.Specifications.SiteSpecifications;

public class UnassignedSitesSpecification : BaseSpecification<Site>
{
    public UnassignedSitesSpecification()
        : base(s => s.AssignedEngineerId == null && s.Status == SiteStatus.OnAir)
    {
        AddOrderBy(s => s.Complexity);
        AddThenBy(s => s.Name);
    }

    public UnassignedSitesSpecification(Guid officeId)
        : base(s => s.AssignedEngineerId == null &&
                    s.Status == SiteStatus.OnAir &&
                    s.OfficeId == officeId)
    {
        AddOrderBy(s => s.Complexity);
        AddThenBy(s => s.Name);
    }
}