using TowerOps.Domain.Entities.Sites;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Specifications;

namespace TowerOps.Domain.Specifications.SiteSpecifications;

public sealed class SitesByComplexitySpecification : BaseSpecification<Site>
{
    public SitesByComplexitySpecification(SiteComplexity complexity, Guid? officeId = null)
        : base(s => s.Complexity == complexity && 
                    (!officeId.HasValue || s.OfficeId == officeId.Value) && 
                    !s.IsDeleted)
    {
        ApplyOrderBy(s => s.Name);
    }
}
