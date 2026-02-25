namespace TowerOps.Application.Commands.Visits.CheckInVisit;

using TowerOps.Application.Common;

public record CheckInVisitCommand : ICommand
{
    public Guid VisitId { get; init; }
    public decimal Latitude { get; init; }
    public decimal Longitude { get; init; }
}
