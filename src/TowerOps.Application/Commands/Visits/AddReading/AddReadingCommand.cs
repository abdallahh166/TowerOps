namespace TowerOps.Application.Commands.Visits.AddReading;

using System;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Visits;

public record AddReadingCommand : ICommand<VisitReadingDto>
{
    public Guid VisitId { get; init; }
    public string ReadingType { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public decimal Value { get; init; }
    public string Unit { get; init; } = string.Empty;
    public decimal? MinAcceptable { get; init; }
    public decimal? MaxAcceptable { get; init; }
    public string? Phase { get; init; }
    public string? Equipment { get; init; }
    public string? Notes { get; init; }
}