namespace TelecomPM.Application.Commands.Visits.SubmitVisit;

using System;
using TelecomPM.Application.Common;

public record SubmitVisitCommand : ICommand
{
    public Guid VisitId { get; init; }
}