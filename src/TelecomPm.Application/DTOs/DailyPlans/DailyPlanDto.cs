using TelecomPM.Domain.Enums;

namespace TelecomPM.Application.DTOs.DailyPlans;

public sealed record DailyPlanDto
{
    public Guid Id { get; init; }
    public Guid OfficeId { get; init; }
    public DateOnly PlanDate { get; init; }
    public Guid OfficeManagerId { get; init; }
    public DailyPlanStatus Status { get; init; }
    public IReadOnlyList<EngineerDayPlanDto> EngineerPlans { get; init; } = Array.Empty<EngineerDayPlanDto>();
}

public sealed record EngineerDayPlanDto
{
    public Guid EngineerId { get; init; }
    public decimal TotalEstimatedDistanceKm { get; init; }
    public int TotalEstimatedTravelMinutes { get; init; }
    public IReadOnlyList<PlannedVisitStopDto> Stops { get; init; } = Array.Empty<PlannedVisitStopDto>();
}

public sealed record PlannedVisitStopDto
{
    public int Order { get; init; }
    public string SiteCode { get; init; } = string.Empty;
    public decimal Latitude { get; init; }
    public decimal Longitude { get; init; }
    public VisitType VisitType { get; init; }
    public string Priority { get; init; } = string.Empty;
    public decimal DistanceFromPreviousKm { get; init; }
    public int EstimatedTravelMinutes { get; init; }
}

public sealed record UnassignedSiteDto
{
    public Guid SiteId { get; init; }
    public string SiteCode { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
}
