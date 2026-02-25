using TowerOps.Application.Common;

namespace TowerOps.Application.Commands.Roles.DeleteApplicationRole;

public sealed record DeleteApplicationRoleCommand : ICommand
{
    public string Id { get; init; } = string.Empty;
}
