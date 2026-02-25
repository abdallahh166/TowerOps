using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Roles;

namespace TowerOps.Application.Queries.Roles.GetAllApplicationRoles;

public sealed record GetAllApplicationRolesQuery : IQuery<IReadOnlyList<ApplicationRoleDto>>
{
    public int? PageNumber { get; init; }
    public int? PageSize { get; init; }
}
