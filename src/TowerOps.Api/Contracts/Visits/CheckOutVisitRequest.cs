namespace TowerOps.Api.Contracts.Visits;

public sealed class CheckOutVisitRequest
{
    public decimal Latitude { get; init; }
    public decimal Longitude { get; init; }
}
