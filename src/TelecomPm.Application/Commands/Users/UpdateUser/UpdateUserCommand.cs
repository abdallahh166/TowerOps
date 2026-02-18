namespace TelecomPM.Application.Commands.Users.UpdateUser;

using System;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Users;

public record UpdateUserCommand : ICommand<UserDto>
{
    public Guid UserId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
}

