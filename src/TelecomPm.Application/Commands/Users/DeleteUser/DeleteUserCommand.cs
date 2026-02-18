namespace TelecomPM.Application.Commands.Users.DeleteUser;

using System;
using TelecomPM.Application.Common;

public record DeleteUserCommand : ICommand
{
    public Guid UserId { get; init; }
    public string DeletedBy { get; init; } = string.Empty;
}

