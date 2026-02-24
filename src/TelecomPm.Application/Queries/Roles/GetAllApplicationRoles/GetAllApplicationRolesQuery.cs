using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Roles;

namespace TelecomPM.Application.Queries.Roles.GetAllApplicationRoles;

public sealed record GetAllApplicationRolesQuery : IQuery<IReadOnlyList<ApplicationRoleDto>>
{
    public int? PageNumber { get; init; }
    public int? PageSize { get; init; }
}
