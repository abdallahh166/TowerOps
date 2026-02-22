using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.DailyPlans;
using TelecomPM.Domain.Enums;

namespace TelecomPM.Application.Commands.DailyPlans.AssignSiteToEngineer;

public sealed record AssignSiteToEngineerCommand : ICommand<DailyPlanDto>
{
    public Guid PlanId { get; init; }
    public Guid EngineerId { get; init; }
    public string SiteCode { get; init; } = string.Empty;
    public VisitType VisitType { get; init; }
    public string Priority { get; init; } = "P3";
}
