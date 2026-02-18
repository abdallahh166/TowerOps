namespace TelecomPM.Application.Commands.Users.ActivateUser;

using System;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Users;

public record ActivateUserCommand : ICommand<UserDto>
{
    public Guid UserId { get; init; }
}

