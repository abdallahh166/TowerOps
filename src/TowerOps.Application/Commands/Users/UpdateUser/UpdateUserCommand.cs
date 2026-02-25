namespace TowerOps.Application.Commands.Users.UpdateUser;

using System;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Users;

public record UpdateUserCommand : ICommand<UserDto>
{
    public Guid UserId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
}

