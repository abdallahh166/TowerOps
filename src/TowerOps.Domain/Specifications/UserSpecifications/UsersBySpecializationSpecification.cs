using TowerOps.Domain.Entities.Users;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Specifications;
using System.Linq;

namespace TowerOps.Domain.Specifications.UserSpecifications;

public sealed class UsersBySpecializationSpecification : BaseSpecification<User>
{
    public UsersBySpecializationSpecification(string specialization, Guid? officeId = null)
        : base(u => u.Role == UserRole.PMEngineer && 
                   (!officeId.HasValue || u.OfficeId == officeId.Value) &&
                   u.IsActive && 
                   !u.IsDeleted &&
                   u.Specializations.Any(s => s.ToUpper() == specialization.ToUpper()))
    {
        AddOrderBy(u => u.Name);
    }
}

