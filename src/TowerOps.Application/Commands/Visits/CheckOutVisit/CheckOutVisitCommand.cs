namespace TowerOps.Application.Commands.Visits.CheckOutVisit;

using TowerOps.Application.Common;

public record CheckOutVisitCommand : ICommand
{
    public Guid VisitId { get; init; }
    public decimal Latitude { get; init; }
    public decimal Longitude { get; init; }
}
