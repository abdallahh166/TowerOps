namespace TelecomPm.Api.Contracts.Materials;

using System.ComponentModel.DataAnnotations;
using TelecomPM.Domain.Enums;

public record AddStockRequest
{
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public decimal Quantity { get; init; }

    [Required]
    public MaterialUnit Unit { get; init; }

    [StringLength(200)]
    public string? Supplier { get; init; }
}
