namespace TowerOps.Application.Queries.Sites.GetSitesByEngineer;

using System;
using System.Collections.Generic;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Sites;

public record GetSitesByEngineerQuery : IQuery<List<SiteDto>>
{
    public Guid EngineerId { get; init; }
}

