namespace TowerOps.Application.Commands.Visits.RescheduleVisit;

using System;
using TowerOps.Application.Common;

public record RescheduleVisitCommand : ICommand
{
    public Guid VisitId { get; init; }
    public DateTime NewScheduledDate { get; init; }
    public string? Reason { get; init; }
}

