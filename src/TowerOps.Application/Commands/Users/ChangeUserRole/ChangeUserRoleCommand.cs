namespace TowerOps.Application.Commands.Users.ChangeUserRole;

using System;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Users;
using TowerOps.Domain.Enums;

public record ChangeUserRoleCommand : ICommand<UserDto>
{
    public Guid UserId { get; init; }
    public UserRole NewRole { get; init; }
}

