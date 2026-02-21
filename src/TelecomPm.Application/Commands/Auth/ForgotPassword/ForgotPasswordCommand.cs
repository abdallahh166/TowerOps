namespace TelecomPM.Application.Commands.Auth.ForgotPassword;

using TelecomPM.Application.Common;

public sealed record ForgotPasswordCommand : ICommand
{
    public string Email { get; init; } = string.Empty;
}
