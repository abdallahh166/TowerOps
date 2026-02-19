namespace TelecomPM.Application.DTOs.Auth;

public sealed class AuthTokenDto
{
    public string AccessToken { get; init; } = string.Empty;
    public DateTime ExpiresAtUtc { get; init; }
    public Guid UserId { get; init; }
    public string Email { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public Guid OfficeId { get; init; }
}
