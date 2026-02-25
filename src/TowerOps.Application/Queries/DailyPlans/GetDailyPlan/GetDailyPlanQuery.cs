using TowerOps.Application.Common;
using TowerOps.Application.DTOs.DailyPlans;

namespace TowerOps.Application.Queries.DailyPlans.GetDailyPlan;

public sealed record GetDailyPlanQuery : IQuery<DailyPlanDto>
{
    public Guid OfficeId { get; init; }
    public DateOnly Date { get; init; }
}
