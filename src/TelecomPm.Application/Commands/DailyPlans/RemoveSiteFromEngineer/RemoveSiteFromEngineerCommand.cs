using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.DailyPlans;

namespace TelecomPM.Application.Commands.DailyPlans.RemoveSiteFromEngineer;

public sealed record RemoveSiteFromEngineerCommand : ICommand<DailyPlanDto>
{
    public Guid PlanId { get; init; }
    public Guid EngineerId { get; init; }
    public string SiteCode { get; init; } = string.Empty;
}
