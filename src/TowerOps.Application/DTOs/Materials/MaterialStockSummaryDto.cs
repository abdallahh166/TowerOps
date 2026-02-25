namespace TowerOps.Application.DTOs.Materials;

using System;
using TowerOps.Domain.Enums;

public record MaterialStockSummaryDto
{
    public Guid MaterialId { get; init; }
    public string MaterialCode { get; init; } = string.Empty;
    public string MaterialName { get; init; } = string.Empty;
    public MaterialCategory Category { get; init; }
    public decimal CurrentStock { get; init; }
    public string Unit { get; init; } = string.Empty;
    public decimal MinimumStock { get; init; }
    public decimal? ReorderQuantity { get; init; }
    public bool IsLowStock { get; init; }
    public decimal StockValue { get; init; }
    public string Currency { get; init; } = string.Empty;
    public DateTime? LastRestockDate { get; init; }
    public int ActiveReservations { get; init; }
}

