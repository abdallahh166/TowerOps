namespace TowerOps.Application.Queries.Sites.GetOfficeSites;

using System;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Sites;
using TowerOps.Domain.Enums;

public record GetOfficeSitesQuery : IQuery<PaginatedList<SiteDto>>
{
    public Guid OfficeId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public SiteComplexity? Complexity { get; init; }
    public SiteStatus? Status { get; init; }
}