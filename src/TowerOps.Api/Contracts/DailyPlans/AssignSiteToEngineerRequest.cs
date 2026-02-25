using TowerOps.Domain.Enums;

namespace TowerOps.Api.Contracts.DailyPlans;

public sealed class AssignSiteToEngineerRequest
{
    public Guid EngineerId { get; init; }
    public string SiteCode { get; init; } = string.Empty;
    public VisitType VisitType { get; init; }
    public string Priority { get; init; } = "P3";
}
