namespace TowerOps.Api.Contracts.Visits;

using System.ComponentModel.DataAnnotations;

public record AddChecklistItemRequest
{
    [Required]
    [MaxLength(100)]
    public string Category { get; init; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string ItemName { get; init; } = string.Empty;

    [Required]
    [MaxLength(1000)]
    public string Description { get; init; } = string.Empty;

    public bool IsMandatory { get; init; }
}
