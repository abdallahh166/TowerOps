namespace TelecomPM.Application.Queries.Offices.GetOfficeById;

using System;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Offices;

public record GetOfficeByIdQuery : IQuery<OfficeDetailDto>
{
    public Guid OfficeId { get; init; }
}

