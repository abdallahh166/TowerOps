namespace TelecomPm.Api.Contracts.Auth;

public sealed class ResetPasswordRequest
{
    public string Email { get; init; } = string.Empty;
    public string Otp { get; init; } = string.Empty;
    public string NewPassword { get; init; } = string.Empty;
}
