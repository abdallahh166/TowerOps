namespace TowerOps.Application.Commands.Users.AssignUserToOffice;

using System;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Users;

public record AssignUserToOfficeCommand : ICommand<UserDto>
{
    public Guid UserId { get; init; }
    public Guid OfficeId { get; init; }
}

