using TowerOps.Api.Contracts.DailyPlans;
using TowerOps.Application.Commands.DailyPlans.AssignSiteToEngineer;
using TowerOps.Application.Commands.DailyPlans.CreateDailyPlan;
using TowerOps.Application.Commands.DailyPlans.PublishDailyPlan;
using TowerOps.Application.Commands.DailyPlans.RemoveSiteFromEngineer;
using TowerOps.Application.Queries.DailyPlans.GetDailyPlan;
using TowerOps.Application.Queries.DailyPlans.GetSuggestedOrder;
using TowerOps.Application.Queries.DailyPlans.GetUnassignedSites;

namespace TowerOps.Api.Mappings;

public static class DailyPlansContractMapper
{
    public static CreateDailyPlanCommand ToCommand(this CreateDailyPlanRequest request)
        => new()
        {
            OfficeId = request.OfficeId,
            PlanDate = request.PlanDate,
            OfficeManagerId = request.OfficeManagerId
        };

    public static GetDailyPlanQuery ToDailyPlanQuery(this (Guid officeId, DateOnly date) value)
        => new() { OfficeId = value.officeId, Date = value.date };

    public static AssignSiteToEngineerCommand ToCommand(this AssignSiteToEngineerRequest request, Guid planId)
        => new()
        {
            PlanId = planId,
            EngineerId = request.EngineerId,
            SiteCode = request.SiteCode,
            VisitType = request.VisitType,
            Priority = request.Priority
        };

    public static RemoveSiteFromEngineerCommand ToCommand(this RemoveSiteFromEngineerRequest request, Guid planId)
        => new()
        {
            PlanId = planId,
            EngineerId = request.EngineerId,
            SiteCode = request.SiteCode
        };

    public static PublishDailyPlanCommand ToPublishCommand(this Guid planId)
        => new() { PlanId = planId };

    public static GetSuggestedOrderQuery ToSuggestedOrderQuery(this (Guid planId, Guid engineerId) value)
        => new() { PlanId = value.planId, EngineerId = value.engineerId };

    public static GetUnassignedSitesQuery ToUnassignedSitesQuery(this (Guid officeId, DateOnly date) value)
        => new() { OfficeId = value.officeId, Date = value.date };
}
