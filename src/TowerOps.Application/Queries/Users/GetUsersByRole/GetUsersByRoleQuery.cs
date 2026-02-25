namespace TowerOps.Application.Queries.Users.GetUsersByRole;

using System;
using System.Collections.Generic;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Users;
using TowerOps.Domain.Enums;

public record GetUsersByRoleQuery : IQuery<List<UserDto>>
{
    public UserRole Role { get; init; }
    public Guid? OfficeId { get; init; }
}

