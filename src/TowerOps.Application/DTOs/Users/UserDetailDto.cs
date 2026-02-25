namespace TowerOps.Application.DTOs.Users;

using System;
using System.Collections.Generic;
using TowerOps.Domain.Enums;

public record UserDetailDto : UserDto
{
    public List<string> Specializations { get; init; } = new();
    public List<Guid> AssignedSiteIds { get; init; } = new();
    public DateTime? LastLoginAt { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

