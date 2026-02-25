namespace TowerOps.Application.Queries.Sites.GetUnassignedSites;

using System;
using System.Collections.Generic;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Sites;

public record GetUnassignedSitesQuery : IQuery<List<SiteDto>>
{
    public Guid? OfficeId { get; init; }
}

