namespace TelecomPM.Application.Commands.Visits.RequestCorrection;

using System;
using TelecomPM.Application.Common;

public record RequestCorrectionCommand : ICommand
{
    public Guid VisitId { get; init; }
    public Guid ReviewerId { get; init; }
    public string CorrectionNotes { get; init; } = string.Empty;
}