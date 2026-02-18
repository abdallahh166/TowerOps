namespace TelecomPm.Api.Contracts.Offices;

using System.ComponentModel.DataAnnotations;

public record UpdateOfficeRequest
{
    [Required]
    [StringLength(200)]
    public string Name { get; init; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Region { get; init; } = string.Empty;

    [Required]
    public UpdateOfficeAddressRequest Address { get; init; } = null!;

    public double? Latitude { get; init; }
    public double? Longitude { get; init; }
}

public record UpdateOfficeAddressRequest
{
    public string? Street { get; init; }

    [Required]
    [StringLength(100)]
    public string City { get; init; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Region { get; init; } = string.Empty;

    [StringLength(500)]
    public string? Details { get; init; }
}

