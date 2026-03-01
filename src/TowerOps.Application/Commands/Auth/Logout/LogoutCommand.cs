using TowerOps.Application.Common;

namespace TowerOps.Application.Commands.Auth.Logout;

public sealed record LogoutCommand : ICommand
{
    public string RefreshToken { get; init; } = string.Empty;
}
