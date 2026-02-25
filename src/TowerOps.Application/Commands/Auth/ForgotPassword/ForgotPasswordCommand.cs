namespace TowerOps.Application.Commands.Auth.ForgotPassword;

using TowerOps.Application.Common;

public sealed record ForgotPasswordCommand : ICommand
{
    public string Email { get; init; } = string.Empty;
}
