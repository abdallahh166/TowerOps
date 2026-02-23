using TelecomPM.Application.Common;

namespace TelecomPM.Application.Commands.Roles.DeleteApplicationRole;

public sealed record DeleteApplicationRoleCommand : ICommand
{
    public string Id { get; init; } = string.Empty;
}
