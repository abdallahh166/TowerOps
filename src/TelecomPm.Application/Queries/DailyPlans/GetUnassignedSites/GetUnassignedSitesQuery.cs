using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.DailyPlans;

namespace TelecomPM.Application.Queries.DailyPlans.GetUnassignedSites;

public sealed record GetUnassignedSitesQuery : IQuery<IReadOnlyList<UnassignedSiteDto>>
{
    public Guid OfficeId { get; init; }
    public DateOnly Date { get; init; }
}
