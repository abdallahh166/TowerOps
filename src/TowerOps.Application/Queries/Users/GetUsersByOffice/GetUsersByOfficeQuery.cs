namespace TowerOps.Application.Queries.Users.GetUsersByOffice;

using System;
using System.Collections.Generic;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Users;

public record GetUsersByOfficeQuery : IQuery<List<UserDto>>
{
    public Guid OfficeId { get; init; }
    public bool? OnlyActive { get; init; }
}

