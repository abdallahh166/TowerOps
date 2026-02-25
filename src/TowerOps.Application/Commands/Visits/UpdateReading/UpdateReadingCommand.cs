namespace TowerOps.Application.Commands.Visits.UpdateReading;

using System;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Visits;

public record UpdateReadingCommand : ICommand<VisitReadingDto>
{
    public Guid VisitId { get; init; }
    public Guid ReadingId { get; init; }
    public decimal Value { get; init; }
}

