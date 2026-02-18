namespace TelecomPM.Application.Commands.Visits.RejectVisit;

using System;
using TelecomPM.Application.Common;

public record RejectVisitCommand : ICommand
{
    public Guid VisitId { get; init; }
    public Guid ReviewerId { get; init; }
    public string RejectionReason { get; init; } = string.Empty;
}