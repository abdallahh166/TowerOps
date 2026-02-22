using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.DailyPlans;

namespace TelecomPM.Application.Commands.DailyPlans.CreateDailyPlan;

public sealed record CreateDailyPlanCommand : ICommand<DailyPlanDto>
{
    public Guid OfficeId { get; init; }
    public DateOnly PlanDate { get; init; }
    public Guid OfficeManagerId { get; init; }
}
