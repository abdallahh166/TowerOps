namespace TelecomPm.Api.Contracts.Offices;

using System.ComponentModel.DataAnnotations;

public record CreateOfficeRequest
{
    [Required]
    [StringLength(10, MinimumLength = 3)]
    public string Code { get; init; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Name { get; init; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Region { get; init; } = string.Empty;

    [Required]
    public CreateOfficeAddressRequest Address { get; init; } = null!;

    public double? Latitude { get; init; }
    public double? Longitude { get; init; }

    [StringLength(200)]
    public string? ContactPerson { get; init; }

    [Phone]
    [StringLength(50)]
    public string? ContactPhone { get; init; }

    [EmailAddress]
    [StringLength(200)]
    public string? ContactEmail { get; init; }
}

public record CreateOfficeAddressRequest
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

