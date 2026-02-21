namespace TelecomPM.Application.Commands.Auth.ResetPassword;

using TelecomPM.Application.Common;

public sealed record ResetPasswordCommand : ICommand
{
    public string Email { get; init; } = string.Empty;
    public string Otp { get; init; } = string.Empty;
    public string NewPassword { get; init; } = string.Empty;
}
