namespace TelecomPM.Application.Commands.Visits.ApproveVisit;

using System;
using TelecomPM.Application.Common;

public record ApproveVisitCommand : ICommand
{
    public Guid VisitId { get; init; }
    public string? Notes { get; init; }
}
