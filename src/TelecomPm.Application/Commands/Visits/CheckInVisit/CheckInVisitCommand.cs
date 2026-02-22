namespace TelecomPM.Application.Commands.Visits.CheckInVisit;

using TelecomPM.Application.Common;

public record CheckInVisitCommand : ICommand
{
    public Guid VisitId { get; init; }
    public Guid EngineerId { get; init; }
    public decimal Latitude { get; init; }
    public decimal Longitude { get; init; }
}
