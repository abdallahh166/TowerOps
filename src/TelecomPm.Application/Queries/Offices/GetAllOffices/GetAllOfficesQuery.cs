namespace TelecomPM.Application.Queries.Offices.GetAllOffices;

using System.Collections.Generic;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Offices;

public record GetAllOfficesQuery : IQuery<List<OfficeDto>>
{
    public bool? OnlyActive { get; init; }
}

