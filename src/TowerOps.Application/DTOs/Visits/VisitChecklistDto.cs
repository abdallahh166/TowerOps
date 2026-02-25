namespace TowerOps.Application.DTOs.Visits;

using System;
using TowerOps.Domain.Enums;

public record VisitChecklistDto
{
    public Guid Id { get; init; }
    public string Category { get; init; } = string.Empty;
    public string ItemName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public CheckStatus Status { get; init; }
    public bool IsMandatory { get; init; }
    public string? Notes { get; init; }
    public DateTime? CompletedAt { get; init; }
}