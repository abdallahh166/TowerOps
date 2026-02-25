using System;
using TowerOps.Domain.Entities.Users;
using TowerOps.Domain.Specifications;

namespace TowerOps.Domain.Specifications.UserSpecifications;

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

