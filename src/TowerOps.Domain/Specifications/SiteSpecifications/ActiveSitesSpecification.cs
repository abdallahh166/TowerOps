using TowerOps.Domain.Entities.Sites;
using TowerOps.Domain.Enums;

namespace TowerOps.Domain.Specifications.SiteSpecifications;

public class ActiveSitesSpecification : BaseSpecification<Site>
{
    public ActiveSitesSpecification()
        : base(s => s.Status == SiteStatus.OnAir)
    {
        AddOrderBy(s => s.Name);
    }

    public ActiveSitesSpecification(Guid officeId)
        : base(s => s.Status == SiteStatus.OnAir && s.OfficeId == officeId)
    {
        AddOrderBy(s => s.Name);
    }
}