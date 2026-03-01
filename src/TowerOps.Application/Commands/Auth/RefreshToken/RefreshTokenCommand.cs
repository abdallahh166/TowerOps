using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Auth;

namespace TowerOps.Application.Commands.Auth.RefreshToken;

public sealed record RefreshTokenCommand : ICommand<AuthTokenDto>
{
    public string RefreshToken { get; init; } = string.Empty;
}
