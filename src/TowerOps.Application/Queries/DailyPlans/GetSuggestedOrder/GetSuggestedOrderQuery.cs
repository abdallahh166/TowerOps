using TowerOps.Application.Common;
using TowerOps.Application.DTOs.DailyPlans;

namespace TowerOps.Application.Queries.DailyPlans.GetSuggestedOrder;

public sealed record GetSuggestedOrderQuery : IQuery<IReadOnlyList<PlannedVisitStopDto>>
{
    public Guid PlanId { get; init; }
    public Guid EngineerId { get; init; }
}
