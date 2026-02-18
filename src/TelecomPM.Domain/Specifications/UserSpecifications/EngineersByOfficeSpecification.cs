using TelecomPM.Domain.Entities.Users;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Specifications;

namespace TelecomPM.Domain.Specifications.UserSpecifications;

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
