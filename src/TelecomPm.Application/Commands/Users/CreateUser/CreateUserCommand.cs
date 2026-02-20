namespace TelecomPM.Application.Commands.Users.CreateUser;

using System;
using System.Collections.Generic;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Users;
using TelecomPM.Domain.Enums;

public record CreateUserCommand : ICommand<UserDto>
{
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public UserRole Role { get; init; }
    public Guid OfficeId { get; init; }
    public int? MaxAssignedSites { get; init; }
    public List<string>? Specializations { get; init; }
}