using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Roles;

namespace TelecomPM.Application.Commands.Roles.CreateApplicationRole;

public sealed record CreateApplicationRoleCommand : ICommand<ApplicationRoleDto>
{
    public string Name { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsActive { get; init; } = true;
    public IReadOnlyList<string> Permissions { get; init; } = Array.Empty<string>();
}
