namespace TowerOps.Application.Queries.Offices.GetAllOffices;

using System.Collections.Generic;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Offices;

public record GetAllOfficesQuery : IQuery<List<OfficeDto>>
{
    public bool? OnlyActive { get; init; }
    public int? PageNumber { get; init; }
    public int? PageSize { get; init; }
}

