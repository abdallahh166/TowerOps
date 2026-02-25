namespace TowerOps.Application.Queries.Sites.GetSitesByComplexity;

using System;
using System.Collections.Generic;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Sites;
using TowerOps.Domain.Enums;

public record GetSitesByComplexityQuery : IQuery<List<SiteDto>>
{
    public SiteComplexity Complexity { get; init; }
    public Guid? OfficeId { get; init; }
}

