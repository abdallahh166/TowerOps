using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.DailyPlans;

namespace TelecomPM.Application.Queries.DailyPlans.GetSuggestedOrder;

public sealed record GetSuggestedOrderQuery : IQuery<IReadOnlyList<PlannedVisitStopDto>>
{
    public Guid PlanId { get; init; }
    public Guid EngineerId { get; init; }
}
