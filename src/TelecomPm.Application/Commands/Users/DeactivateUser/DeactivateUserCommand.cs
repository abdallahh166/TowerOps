namespace TelecomPM.Application.Commands.Users.DeactivateUser;

using System;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Users;

public record DeactivateUserCommand : ICommand<UserDto>
{
    public Guid UserId { get; init; }
}

