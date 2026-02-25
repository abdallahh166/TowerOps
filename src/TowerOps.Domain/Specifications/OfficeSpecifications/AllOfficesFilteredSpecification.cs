using TowerOps.Domain.Entities.Offices;

namespace TowerOps.Domain.Specifications.OfficeSpecifications;

public sealed class AllOfficesFilteredSpecification : BaseSpecification<Office>
{
    public AllOfficesFilteredSpecification(bool onlyActive, int skip, int take)
        : base(o => (!onlyActive || o.IsActive) && !o.IsDeleted)
    {
        ApplyOrderBy(o => o.Code);
        ApplyPaging(skip, take);
    }
}
