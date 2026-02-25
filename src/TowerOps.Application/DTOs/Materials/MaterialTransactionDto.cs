namespace TowerOps.Application.DTOs.Materials;

using System;
using TowerOps.Domain.Entities.Materials;
using TowerOps.Domain.Enums;

public record MaterialTransactionDto
{
    public Guid Id { get; init; }
    public Guid MaterialId { get; init; }
    public Guid? VisitId { get; init; }
    public TransactionType Type { get; init; }
    public decimal Quantity { get; init; }
    public string Unit { get; init; } = string.Empty;
    public decimal StockBefore { get; init; }
    public decimal StockAfter { get; init; }
    public string Reason { get; init; } = string.Empty;
    public DateTime TransactionDate { get; init; }
    public string PerformedBy { get; init; } = string.Empty;
}

