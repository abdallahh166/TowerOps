namespace TowerOps.Application.Commands.Users.ActivateUser;

using System;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Users;

public record ActivateUserCommand : ICommand<UserDto>
{
    public Guid UserId { get; init; }
}

