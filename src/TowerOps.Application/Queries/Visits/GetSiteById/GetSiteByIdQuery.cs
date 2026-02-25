namespace TowerOps.Application.Queries.Sites.GetSiteById;

using System;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Sites;

public record GetSiteByIdQuery : IQuery<SiteDetailDto>
{
    public Guid SiteId { get; init; }
}