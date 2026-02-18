namespace TelecomPM.Application.Queries.Users.GetUsersBySpecialization;

using System;
using System.Collections.Generic;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Users;

public record GetUsersBySpecializationQuery : IQuery<List<UserDto>>
{
    public string Specialization { get; init; } = string.Empty;
    public Guid? OfficeId { get; init; }
}

