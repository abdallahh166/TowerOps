namespace TelecomPm.Api.Contracts.Visits;

public sealed class CheckOutVisitRequest
{
    public Guid EngineerId { get; init; }
    public decimal Latitude { get; init; }
    public decimal Longitude { get; init; }
}
