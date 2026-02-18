namespace TelecomPM.Application.DTOs.Materials;

using System;
using TelecomPM.Domain.Enums;

public record MaterialDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public MaterialCategory Category { get; init; }
    public decimal CurrentStock { get; init; }
    public string Unit { get; init; } = string.Empty;
    public decimal MinimumStock { get; init; }
    public decimal UnitCost { get; init; }
    public string Currency { get; init; } = string.Empty;
    public bool IsLowStock { get; init; }
    public bool IsActive { get; init; }
}