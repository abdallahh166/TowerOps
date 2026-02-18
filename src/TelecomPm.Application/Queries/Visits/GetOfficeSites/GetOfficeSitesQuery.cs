namespace TelecomPM.Application.Queries.Sites.GetOfficeSites;

using System;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Sites;
using TelecomPM.Domain.Enums;

public record GetOfficeSitesQuery : IQuery<PaginatedList<SiteDto>>
{
    public Guid OfficeId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public SiteComplexity? Complexity { get; init; }
    public SiteStatus? Status { get; init; }
}