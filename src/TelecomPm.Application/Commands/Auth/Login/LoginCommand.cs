namespace TelecomPM.Application.Commands.Auth.Login;

using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Auth;

public sealed record LoginCommand : ICommand<AuthTokenDto>
{
    public string Email { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
}
