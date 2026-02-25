namespace TowerOps.Application.Commands.Users.DeactivateUser;

using System;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Users;

public record DeactivateUserCommand : ICommand<UserDto>
{
    public Guid UserId { get; init; }
}

