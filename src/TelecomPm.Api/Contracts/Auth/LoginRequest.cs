namespace TelecomPm.Api.Contracts.Auth;

public sealed class LoginRequest
{
    public string Email { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
}
