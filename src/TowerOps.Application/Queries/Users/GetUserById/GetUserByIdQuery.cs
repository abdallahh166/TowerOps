namespace TowerOps.Application.Queries.Users.GetUserById;

using System;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Users;

public record GetUserByIdQuery : IQuery<UserDetailDto>
{
    public Guid UserId { get; init; }
}

