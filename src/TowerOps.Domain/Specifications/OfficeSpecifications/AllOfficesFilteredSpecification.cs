using TowerOps.Domain.Entities.Offices;

namespace TowerOps.Domain.Specifications.OfficeSpecifications;

public sealed class AllOfficesFilteredSpecification : BaseSpecification<Office>
{
    public AllOfficesFilteredSpecification(
        bool onlyActive,
        int skip,
        int take,
        string sortBy = "code",
        bool sortDescending = false)
        : base(o => (!onlyActive || o.IsActive) && !o.IsDeleted)
    {
        switch (sortBy.Trim().ToLowerInvariant())
        {
            case "name":
                if (sortDescending) ApplyOrderByDescending(o => o.Name);
                else ApplyOrderBy(o => o.Name);
                break;
            case "region":
                if (sortDescending) ApplyOrderByDescending(o => o.Region);
                else ApplyOrderBy(o => o.Region);
                break;
            case "isactive":
                if (sortDescending) ApplyOrderByDescending(o => o.IsActive);
                else ApplyOrderBy(o => o.IsActive);
                break;
            case "createdat":
                if (sortDescending) ApplyOrderByDescending(o => o.CreatedAt);
                else ApplyOrderBy(o => o.CreatedAt);
                break;
            case "code":
            default:
                if (sortDescending) ApplyOrderByDescending(o => o.Code);
                else ApplyOrderBy(o => o.Code);
                break;
        }

        ApplyPaging(skip, take);
    }
}
