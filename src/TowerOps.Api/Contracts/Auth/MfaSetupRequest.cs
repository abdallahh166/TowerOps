namespace TowerOps.Api.Contracts.Auth;

public sealed class MfaSetupRequest
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

