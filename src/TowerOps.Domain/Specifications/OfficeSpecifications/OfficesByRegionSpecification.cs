using TowerOps.Domain.Entities.Offices;
using TowerOps.Domain.Specifications;

namespace TowerOps.Domain.Specifications.OfficeSpecifications;

public sealed class OfficesByRegionSpecification : BaseSpecification<Office>
{
    public OfficesByRegionSpecification(string region, bool onlyActive = true)
        : base(o => o.Region == region &&
                    (!onlyActive || o.IsActive) &&
                    !o.IsDeleted)
    {
        ApplyOrderBy(o => o.Name);
    }
}

