using TelecomPM.Domain.Entities.Users;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Specifications;

namespace TelecomPM.Domain.Specifications.UserSpecifications;

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
