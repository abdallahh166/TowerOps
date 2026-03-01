using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Auth;

namespace TowerOps.Application.Commands.Auth.GenerateMfaSetup;

public sealed record GenerateMfaSetupCommand : ICommand<MfaSetupDto>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
