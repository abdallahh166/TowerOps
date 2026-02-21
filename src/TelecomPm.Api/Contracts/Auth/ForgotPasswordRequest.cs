namespace TelecomPm.Api.Contracts.Auth;

public sealed class ForgotPasswordRequest
{
    public string Email { get; init; } = string.Empty;
}
