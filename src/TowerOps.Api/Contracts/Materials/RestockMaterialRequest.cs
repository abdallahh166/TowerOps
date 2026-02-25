namespace TowerOps.Api.Contracts.Materials;

using System.ComponentModel.DataAnnotations;

public record RestockMaterialRequest
{
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public decimal Quantity { get; init; }

    [Required]
    [StringLength(500)]
    public string Reason { get; init; } = string.Empty;
}

