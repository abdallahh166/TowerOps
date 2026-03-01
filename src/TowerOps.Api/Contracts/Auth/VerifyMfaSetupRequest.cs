namespace TowerOps.Api.Contracts.Auth;

public sealed class VerifyMfaSetupRequest
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
}
