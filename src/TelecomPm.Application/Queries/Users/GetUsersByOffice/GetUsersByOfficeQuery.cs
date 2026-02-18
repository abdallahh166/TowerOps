namespace TelecomPM.Application.Queries.Users.GetUsersByOffice;

using System;
using System.Collections.Generic;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Users;

public record GetUsersByOfficeQuery : IQuery<List<UserDto>>
{
    public Guid OfficeId { get; init; }
    public bool? OnlyActive { get; init; }
}

