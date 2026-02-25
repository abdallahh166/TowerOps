namespace TowerOps.Application.Commands.Users.DeleteUser;

using System;
using TowerOps.Application.Common;

public record DeleteUserCommand : ICommand
{
    public Guid UserId { get; init; }
    public string DeletedBy { get; init; } = string.Empty;
}

