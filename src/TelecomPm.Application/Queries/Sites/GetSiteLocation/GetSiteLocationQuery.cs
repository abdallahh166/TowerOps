namespace TelecomPM.Application.Queries.Sites.GetSiteLocation;

using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Sites;

public sealed record GetSiteLocationQuery : IQuery<SiteLocationDto>
{
    public string SiteCode { get; init; } = string.Empty;
}
