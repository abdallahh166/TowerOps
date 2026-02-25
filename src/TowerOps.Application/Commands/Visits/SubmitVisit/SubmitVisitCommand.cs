namespace TowerOps.Application.Commands.Visits.SubmitVisit;

using System;
using TowerOps.Application.Common;

public record SubmitVisitCommand : ICommand
{
    public Guid VisitId { get; init; }
}