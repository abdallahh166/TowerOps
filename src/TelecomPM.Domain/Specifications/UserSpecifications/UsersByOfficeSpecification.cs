using System;
using TelecomPM.Domain.Entities.Users;
using TelecomPM.Domain.Specifications;

namespace TelecomPM.Domain.Specifications.UserSpecifications;

public sealed class UsersByOfficeSpecification : BaseSpecification<User>
{
    public UsersByOfficeSpecification(Guid officeId, bool onlyActive = true)
        : base(u => u.OfficeId == officeId &&
                    (!onlyActive || u.IsActive) &&
                    !u.IsDeleted)
    {
        ApplyOrderBy(u => u.Name);
    }
}

