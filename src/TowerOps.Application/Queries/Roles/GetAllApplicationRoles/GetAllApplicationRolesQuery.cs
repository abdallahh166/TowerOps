using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Roles;

namespace TowerOps.Application.Queries.Roles.GetAllApplicationRoles;

public sealed record GetAllApplicationRolesQuery : IQuery<PaginatedList<ApplicationRoleDto>>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 25;
    public string? SortBy { get; init; }
    public bool SortDescending { get; init; } = true;
}
