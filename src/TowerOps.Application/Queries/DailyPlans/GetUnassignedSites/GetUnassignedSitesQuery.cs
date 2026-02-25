using TowerOps.Application.Common;
using TowerOps.Application.DTOs.DailyPlans;

namespace TowerOps.Application.Queries.DailyPlans.GetUnassignedSites;

public sealed record GetUnassignedSitesQuery : IQuery<IReadOnlyList<UnassignedSiteDto>>
{
    public Guid OfficeId { get; init; }
    public DateOnly Date { get; init; }
}
