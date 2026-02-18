namespace TelecomPM.Application.Commands.Users.ChangeUserRole;

using System;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Users;
using TelecomPM.Domain.Enums;

public record ChangeUserRoleCommand : ICommand<UserDto>
{
    public Guid UserId { get; init; }
    public UserRole NewRole { get; init; }
}

