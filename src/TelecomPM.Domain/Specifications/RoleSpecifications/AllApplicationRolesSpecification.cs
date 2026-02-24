using TelecomPM.Domain.Entities.ApplicationRoles;

namespace TelecomPM.Domain.Specifications.RoleSpecifications;

public sealed class AllApplicationRolesSpecification : BaseSpecification<ApplicationRole>
{
    public AllApplicationRolesSpecification(int skip, int take)
        : base(r => !r.IsDeleted)
    {
        ApplyOrderBy(r => r.Name);
        ApplyPaging(skip, take);
    }
}
