using TowerOps.Domain.Entities.Sites;
using TowerOps.Domain.Specifications;

namespace TowerOps.Domain.Specifications.SiteSpecifications;

public sealed class SitesByOfficeSpecification : BaseSpecification<Site>
{
    public SitesByOfficeSpecification(Guid officeId)
        : base(s => s.OfficeId == officeId && !s.IsDeleted)
    {
        AddInclude(s => s.PowerSystem);
        AddInclude(s => s.RadioEquipment);
        ApplyOrderBy(s => s.SiteCode.Value);
    }
}
