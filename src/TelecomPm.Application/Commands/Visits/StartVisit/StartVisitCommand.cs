namespace TelecomPM.Application.Commands.Visits.StartVisit;

using System;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Visits;

public record StartVisitCommand : ICommand<VisitDto>
{
    public Guid VisitId { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }
}