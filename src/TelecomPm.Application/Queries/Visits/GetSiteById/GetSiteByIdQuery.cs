namespace TelecomPM.Application.Queries.Sites.GetSiteById;

using System;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Sites;

public record GetSiteByIdQuery : IQuery<SiteDetailDto>
{
    public Guid SiteId { get; init; }
}