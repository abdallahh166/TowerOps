using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.DailyPlans;

namespace TelecomPM.Application.Commands.DailyPlans.PublishDailyPlan;

public sealed record PublishDailyPlanCommand : ICommand<DailyPlanDto>
{
    public Guid PlanId { get; init; }
}
