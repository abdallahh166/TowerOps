namespace TelecomPM.Application.Queries.Sites.GetSitesByComplexity;

using System;
using System.Collections.Generic;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Sites;
using TelecomPM.Domain.Enums;

public record GetSitesByComplexityQuery : IQuery<List<SiteDto>>
{
    public SiteComplexity Complexity { get; init; }
    public Guid? OfficeId { get; init; }
}

