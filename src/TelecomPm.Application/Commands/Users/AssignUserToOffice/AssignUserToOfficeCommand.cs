namespace TelecomPM.Application.Commands.Users.AssignUserToOffice;

using System;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Users;

public record AssignUserToOfficeCommand : ICommand<UserDto>
{
    public Guid UserId { get; init; }
    public Guid OfficeId { get; init; }
}

