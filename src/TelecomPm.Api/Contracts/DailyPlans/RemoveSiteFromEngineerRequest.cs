namespace TelecomPm.Api.Contracts.DailyPlans;

public sealed class RemoveSiteFromEngineerRequest
{
    public Guid EngineerId { get; init; }
    public string SiteCode { get; init; } = string.Empty;
}
