namespace TowerOps.Application.Queries.Sites.GetSitesByRegion;

using System.Collections.Generic;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Sites;

public record GetSitesByRegionQuery : IQuery<List<SiteDto>>
{
    public string Region { get; init; } = string.Empty;
    public string? SubRegion { get; init; }
}

