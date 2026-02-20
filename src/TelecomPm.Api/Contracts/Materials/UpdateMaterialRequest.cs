namespace TelecomPm.Api.Contracts.Materials;

using System.ComponentModel.DataAnnotations;
using TelecomPM.Domain.Enums;

public record UpdateMaterialRequest
{
    [Required]
    [StringLength(200)]
    public string Name { get; init; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; init; }

    [Required]
    public MaterialCategory Category { get; init; }

    [StringLength(200)]
    public string? Supplier { get; init; }
}

