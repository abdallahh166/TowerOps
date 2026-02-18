namespace TelecomPm.Api.Contracts.Materials;

using System;
using System.ComponentModel.DataAnnotations;
using TelecomPM.Domain.Enums;

public record CreateMaterialRequest
{
    [Required]
    [StringLength(50)]
    [RegularExpression(@"^[A-Z0-9\-]+$", ErrorMessage = "Material code must contain only uppercase letters, numbers, and hyphens")]
    public string Code { get; init; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Name { get; init; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; init; }

    [Required]
    public MaterialCategory Category { get; init; }

    [Required]
    public Guid OfficeId { get; init; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Initial stock must be greater than 0")]
    public decimal InitialStock { get; init; }

    [Required]
    public MaterialUnit Unit { get; init; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Minimum stock must be greater than 0")]
    public decimal MinimumStock { get; init; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Unit cost must be greater than 0")]
    public decimal UnitCost { get; init; }

    [StringLength(200)]
    public string? Supplier { get; init; }
}

