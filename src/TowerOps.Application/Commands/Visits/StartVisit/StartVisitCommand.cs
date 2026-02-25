namespace TowerOps.Application.Commands.Visits.StartVisit;

using System;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Visits;

public record StartVisitCommand : ICommand<VisitDto>
{
    public Guid VisitId { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }
}