namespace TelecomPM.Application.Commands.Visits.CompleteVisit;

using System;
using TelecomPM.Application.Common;

public record CompleteVisitCommand : ICommand
{
    public Guid VisitId { get; init; }
    public string? EngineerNotes { get; init; }
}