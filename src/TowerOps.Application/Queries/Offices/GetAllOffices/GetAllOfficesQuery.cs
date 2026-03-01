namespace TowerOps.Application.Queries.Offices.GetAllOffices;

using System.Collections.Generic;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Offices;

public record GetAllOfficesQuery : IQuery<PaginatedList<OfficeDto>>
{
    public bool? OnlyActive { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 25;
    public string? SortBy { get; init; }
    public bool SortDescending { get; init; } = true;
}

