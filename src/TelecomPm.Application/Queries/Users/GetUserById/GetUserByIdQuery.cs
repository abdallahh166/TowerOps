namespace TelecomPM.Application.Queries.Users.GetUserById;

using System;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Users;

public record GetUserByIdQuery : IQuery<UserDetailDto>
{
    public Guid UserId { get; init; }
}

