using TowerOps.Application.Common;
using TowerOps.Application.DTOs.DailyPlans;

namespace TowerOps.Application.Commands.DailyPlans.CreateDailyPlan;

public sealed record CreateDailyPlanCommand : ICommand<DailyPlanDto>
{
    public Guid OfficeId { get; init; }
    public DateOnly PlanDate { get; init; }
    public Guid OfficeManagerId { get; init; }
}
