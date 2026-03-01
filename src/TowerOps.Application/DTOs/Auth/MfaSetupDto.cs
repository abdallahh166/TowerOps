namespace TowerOps.Application.DTOs.Auth;

public sealed record MfaSetupDto
{
    public string Secret { get; init; } = string.Empty;
    public string OtpAuthUri { get; init; } = string.Empty;
}

