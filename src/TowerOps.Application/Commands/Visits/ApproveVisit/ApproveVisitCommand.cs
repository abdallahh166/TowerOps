namespace TowerOps.Application.Commands.Visits.ApproveVisit;

using System;
using TowerOps.Application.Common;

public record ApproveVisitCommand : ICommand
{
    public Guid VisitId { get; init; }
    public string? Notes { get; init; }
}
