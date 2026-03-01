using TowerOps.Domain.Entities.ApplicationRoles;

namespace TowerOps.Domain.Specifications.RoleSpecifications;

public sealed class AllApplicationRolesSpecification : BaseSpecification<ApplicationRole>
{
    public AllApplicationRolesSpecification(
        int skip,
        int take,
        string sortBy = "name",
        bool sortDescending = false)
        : base(r => !r.IsDeleted)
    {
        switch (sortBy.Trim().ToLowerInvariant())
        {
            case "displayname":
                if (sortDescending) ApplyOrderByDescending(r => r.DisplayName);
                else ApplyOrderBy(r => r.DisplayName);
                break;
            case "isactive":
                if (sortDescending) ApplyOrderByDescending(r => r.IsActive);
                else ApplyOrderBy(r => r.IsActive);
                break;
            case "issystem":
                if (sortDescending) ApplyOrderByDescending(r => r.IsSystem);
                else ApplyOrderBy(r => r.IsSystem);
                break;
            case "updatedat":
                if (sortDescending) ApplyOrderByDescending(r => r.UpdatedAt);
                else ApplyOrderBy(r => r.UpdatedAt);
                break;
            case "name":
            default:
                if (sortDescending) ApplyOrderByDescending(r => r.Name);
                else ApplyOrderBy(r => r.Name);
                break;
        }

        ApplyPaging(skip, take);
    }
}
