using TowerOps.Application.Common;

namespace TowerOps.Application.Commands.Auth.VerifyMfaSetup;

public sealed record VerifyMfaSetupCommand : ICommand
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
}
