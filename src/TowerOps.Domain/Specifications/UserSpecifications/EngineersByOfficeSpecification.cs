using TowerOps.Domain.Entities.Users;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Specifications;

namespace TowerOps.Domain.Specifications.UserSpecifications;

public sealed class EngineersByOfficeSpecification : BaseSpecification<User>
{
    public EngineersByOfficeSpecification(Guid officeId)
        : base(u => u.OfficeId == officeId && 
                    u.Role == UserRole.PMEngineer && 
                    u.IsActive && 
                    !u.IsDeleted)
    {
        ApplyOrderBy(u => u.Name);
    }
}
