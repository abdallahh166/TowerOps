namespace TowerOps.Application.Commands.Visits.RejectVisit;

using System;
using TowerOps.Application.Common;

public record RejectVisitCommand : ICommand
{
    public Guid VisitId { get; init; }
    public string RejectionReason { get; init; } = string.Empty;
}
