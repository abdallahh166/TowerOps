namespace TelecomPM.Application.Queries.Sites.GetSitesByEngineer;

using System;
using System.Collections.Generic;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Sites;

public record GetSitesByEngineerQuery : IQuery<List<SiteDto>>
{
    public Guid EngineerId { get; init; }
}

