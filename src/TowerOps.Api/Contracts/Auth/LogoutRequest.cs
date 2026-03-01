namespace TowerOps.Api.Contracts.Auth;

public sealed class LogoutRequest
{
    public string RefreshToken { get; init; } = string.Empty;
}
