namespace TelecomPM.Application.Commands.Visits.CancelVisit;

using System;
using TelecomPM.Application.Common;

public record CancelVisitCommand : ICommand
{
    public Guid VisitId { get; init; }
    public string Reason { get; init; } = string.Empty;
}

