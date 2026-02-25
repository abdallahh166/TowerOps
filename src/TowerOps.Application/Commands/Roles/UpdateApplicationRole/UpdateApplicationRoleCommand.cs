using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Roles;

namespace TowerOps.Application.Commands.Roles.UpdateApplicationRole;

public sealed record UpdateApplicationRoleCommand : ICommand<ApplicationRoleDto>
{
    public string Id { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsActive { get; init; } = true;
    public IReadOnlyList<string> Permissions { get; init; } = Array.Empty<string>();
}
