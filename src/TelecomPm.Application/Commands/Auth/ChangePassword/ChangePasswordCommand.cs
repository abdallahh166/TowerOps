namespace TelecomPM.Application.Commands.Auth.ChangePassword;

using TelecomPM.Application.Common;

public sealed record ChangePasswordCommand : ICommand
{
    public string CurrentPassword { get; init; } = string.Empty;
    public string NewPassword { get; init; } = string.Empty;
    public string ConfirmPassword { get; init; } = string.Empty;
}
