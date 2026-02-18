namespace TelecomPM.Application.Commands.Visits.UpdateReading;

using System;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Visits;

public record UpdateReadingCommand : ICommand<VisitReadingDto>
{
    public Guid VisitId { get; init; }
    public Guid ReadingId { get; init; }
    public decimal Value { get; init; }
}

