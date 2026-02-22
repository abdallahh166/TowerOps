namespace TelecomPm.Api.Contracts.Visits;

public sealed class CheckInVisitRequest
{
    public decimal Latitude { get; init; }
    public decimal Longitude { get; init; }
}
