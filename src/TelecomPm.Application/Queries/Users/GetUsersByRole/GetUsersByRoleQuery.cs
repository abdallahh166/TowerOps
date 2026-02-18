namespace TelecomPM.Application.Queries.Users.GetUsersByRole;

using System;
using System.Collections.Generic;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Users;
using TelecomPM.Domain.Enums;

public record GetUsersByRoleQuery : IQuery<List<UserDto>>
{
    public UserRole Role { get; init; }
    public Guid? OfficeId { get; init; }
}

