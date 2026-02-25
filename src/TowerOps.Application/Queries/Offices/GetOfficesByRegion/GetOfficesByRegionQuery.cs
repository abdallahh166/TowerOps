namespace TowerOps.Application.Queries.Offices.GetOfficesByRegion;

using System.Collections.Generic;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Offices;

public record GetOfficesByRegionQuery : IQuery<List<OfficeDto>>
{
    public string Region { get; init; } = string.Empty;
    public bool? OnlyActive { get; init; }
}

