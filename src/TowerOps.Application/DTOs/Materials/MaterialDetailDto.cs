namespace TowerOps.Application.DTOs.Materials;

using System;
using System.Collections.Generic;
using TowerOps.Domain.Enums;

public record MaterialDetailDto : MaterialDto
{
    public string? Supplier { get; init; }
    public DateTime? LastRestockDate { get; init; }
    public decimal? ReorderQuantity { get; init; }
    public List<MaterialTransactionDto> RecentTransactions { get; init; } = new();
    public List<MaterialReservationDto> ActiveReservations { get; init; } = new();
}

