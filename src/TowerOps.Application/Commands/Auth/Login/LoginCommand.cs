namespace TowerOps.Application.Commands.Auth.Login;

using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Auth;

public sealed record LoginCommand : ICommand<AuthTokenDto>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string? MfaCode { get; init; }
}
