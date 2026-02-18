namespace TelecomPM.Application.Commands.Visits.RescheduleVisit;

using System;
using TelecomPM.Application.Common;

public record RescheduleVisitCommand : ICommand
{
    public Guid VisitId { get; init; }
    public DateTime NewScheduledDate { get; init; }
    public string? Reason { get; init; }
}

