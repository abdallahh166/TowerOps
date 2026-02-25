namespace TowerOps.Application.Commands.Visits.CompleteVisit;

using System;
using TowerOps.Application.Common;

public record CompleteVisitCommand : ICommand
{
    public Guid VisitId { get; init; }
    public string? EngineerNotes { get; init; }
}