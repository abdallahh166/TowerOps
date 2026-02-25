using TowerOps.Application.Common;
using TowerOps.Application.DTOs.DailyPlans;

namespace TowerOps.Application.Commands.DailyPlans.PublishDailyPlan;

public sealed record PublishDailyPlanCommand : ICommand<DailyPlanDto>
{
    public Guid PlanId { get; init; }
}
