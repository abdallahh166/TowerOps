namespace TowerOps.Api.Contracts.Auth;

public sealed class MfaSetupResponse
{
    public string Secret { get; init; } = string.Empty;
    public string OtpAuthUri { get; init; } = string.Empty;
}

