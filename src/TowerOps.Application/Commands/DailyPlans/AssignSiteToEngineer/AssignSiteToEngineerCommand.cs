using TowerOps.Application.Common;
using TowerOps.Application.DTOs.DailyPlans;
using TowerOps.Domain.Enums;

namespace TowerOps.Application.Commands.DailyPlans.AssignSiteToEngineer;

public sealed record AssignSiteToEngineerCommand : ICommand<DailyPlanDto>
{
    public Guid PlanId { get; init; }
    public Guid EngineerId { get; init; }
    public string SiteCode { get; init; } = string.Empty;
    public VisitType VisitType { get; init; }
    public string Priority { get; init; } = "P3";
}
