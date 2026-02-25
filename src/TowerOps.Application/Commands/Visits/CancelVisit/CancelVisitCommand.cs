namespace TowerOps.Application.Commands.Visits.CancelVisit;

using System;
using TowerOps.Application.Common;

public record CancelVisitCommand : ICommand
{
    public Guid VisitId { get; init; }
    public string Reason { get; init; } = string.Empty;
}

