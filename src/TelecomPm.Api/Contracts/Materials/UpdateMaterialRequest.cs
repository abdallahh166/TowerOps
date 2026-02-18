namespace TelecomPm.Api.Contracts.Materials;

using System.ComponentModel.DataAnnotations;

public record UpdateMaterialRequest
{
    [Required]
    [StringLength(200)]
    public string Name { get; init; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; init; }

    [StringLength(200)]
    public string? Supplier { get; init; }
}

