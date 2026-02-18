namespace TelecomPM.Application.Queries.Offices.GetOfficesByRegion;

using System.Collections.Generic;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Offices;

public record GetOfficesByRegionQuery : IQuery<List<OfficeDto>>
{
    public string Region { get; init; } = string.Empty;
    public bool? OnlyActive { get; init; }
}

