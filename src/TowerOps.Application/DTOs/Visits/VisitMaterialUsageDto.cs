namespace TowerOps.Application.DTOs.Visits;

using System;
using TowerOps.Domain.Enums;

public record VisitMaterialUsageDto
{
    public Guid Id { get; init; }
    public string MaterialCode { get; init; } = string.Empty;
    public string MaterialName { get; init; } = string.Empty;
    public MaterialCategory Category { get; init; }
    public decimal Quantity { get; init; }
    public string Unit { get; init; } = string.Empty;
    public decimal UnitCost { get; init; }
    public decimal TotalCost { get; init; }
    public string Reason { get; init; } = string.Empty;
    public string? BeforePhotoUrl { get; init; }
    public string? AfterPhotoUrl { get; init; }
    public MaterialUsageStatus Status { get; init; }
    public DateTime UsedAt { get; init; }
}