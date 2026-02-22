namespace TelecomPm.Api.Contracts.DailyPlans;

public sealed class CreateDailyPlanRequest
{
    public Guid OfficeId { get; init; }
    public DateOnly PlanDate { get; init; }
    public Guid OfficeManagerId { get; init; }
}
