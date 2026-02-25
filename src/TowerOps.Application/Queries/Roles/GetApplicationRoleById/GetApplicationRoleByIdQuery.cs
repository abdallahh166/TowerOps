using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Roles;

namespace TowerOps.Application.Queries.Roles.GetApplicationRoleById;

public sealed record GetApplicationRoleByIdQuery : IQuery<ApplicationRoleDto>
{
    public string Id { get; init; } = string.Empty;
}
