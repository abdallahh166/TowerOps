namespace TowerOps.Application.Commands.Users.UpdateUserSpecializations;

using System;
using System.Collections.Generic;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Users;

public record UpdateUserSpecializationsCommand : ICommand<UserDto>
{
    public Guid UserId { get; init; }
    public int MaxAssignedSites { get; init; }
    public List<string> Specializations { get; init; } = new();
}

