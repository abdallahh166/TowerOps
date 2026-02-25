namespace TowerOps.Application.Commands.Visits.RequestCorrection;

using System;
using TowerOps.Application.Common;

public record RequestCorrectionCommand : ICommand
{
    public Guid VisitId { get; init; }
    public string CorrectionNotes { get; init; } = string.Empty;
}
