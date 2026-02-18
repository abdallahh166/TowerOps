namespace TelecomPM.Application.Queries.Sites.GetSitesByRegion;

using System.Collections.Generic;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Sites;

public record GetSitesByRegionQuery : IQuery<List<SiteDto>>
{
    public string Region { get; init; } = string.Empty;
    public string? SubRegion { get; init; }
}

