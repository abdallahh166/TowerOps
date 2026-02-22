namespace TelecomPm.Api.Contracts.Visits;

public sealed class CheckInVisitRequest
{
    public Guid EngineerId { get; init; }
    public decimal Latitude { get; init; }
    public decimal Longitude { get; init; }
}
