using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.DailyPlans;

namespace TelecomPM.Application.Queries.DailyPlans.GetDailyPlan;

public sealed record GetDailyPlanQuery : IQuery<DailyPlanDto>
{
    public Guid OfficeId { get; init; }
    public DateOnly Date { get; init; }
}
