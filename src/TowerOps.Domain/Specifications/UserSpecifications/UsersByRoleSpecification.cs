using TowerOps.Domain.Entities.Users;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Specifications;

namespace TowerOps.Domain.Specifications.UserSpecifications;

public sealed class UsersByRoleSpecification : BaseSpecification<User>
{
    public UsersByRoleSpecification(UserRole role, Guid? officeId = null)
        : base(u => u.Role == role && 
                    (!officeId.HasValue || u.OfficeId == officeId.Value) && 
                    u.IsActive && 
                    !u.IsDeleted)
    {
        ApplyOrderBy(u => u.Name);
    }
}
