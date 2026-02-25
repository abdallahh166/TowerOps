namespace TowerOps.Application.Queries.Users.GetUsersBySpecialization;

using System;
using System.Collections.Generic;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Users;

public record GetUsersBySpecializationQuery : IQuery<List<UserDto>>
{
    public string Specialization { get; init; } = string.Empty;
    public Guid? OfficeId { get; init; }
}

