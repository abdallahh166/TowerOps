using System;

namespace TowerOps.Application.DTOs.Visits;

public record VisitReadingDto
{
    public Guid Id { get; init; }
    public string ReadingType { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public decimal Value { get; init; }
    public string Unit { get; init; } = string.Empty;
    public decimal? MinAcceptable { get; init; }
    public decimal? MaxAcceptable { get; init; }
    public bool IsWithinRange { get; init; }
    public string? Phase { get; init; }
    public string? Equipment { get; init; }
    public string? Notes { get; init; }
    public DateTime MeasuredAt { get; init; }
}