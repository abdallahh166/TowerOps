namespace TelecomPM.Application.Queries.Sites.GetUnassignedSites;

using System;
using System.Collections.Generic;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Sites;

public record GetUnassignedSitesQuery : IQuery<List<SiteDto>>
{
    public Guid? OfficeId { get; init; }
}

