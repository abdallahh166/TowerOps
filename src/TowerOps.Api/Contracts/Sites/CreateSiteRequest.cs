namespace TowerOps.Api.Contracts.Sites;

using System;
using System.ComponentModel.DataAnnotations;
using TowerOps.Domain.Enums;

public record CreateSiteRequest
{
    [Required]
    [StringLength(50, MinimumLength = 3)]
    [RegularExpression(@"^[A-Z]{3}\d+$", ErrorMessage = "Site code must be in format: XXX123")]
    public string SiteCode { get; init; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Name { get; init; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string OMCName { get; init; } = string.Empty;

    [Required]
    public Guid OfficeId { get; init; }

    [Required]
    [StringLength(100)]
    public string Region { get; init; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string SubRegion { get; init; } = string.Empty;

    [Required]
    [Range(-90, 90)]
    public double Latitude { get; init; }

    [Required]
    [Range(-180, 180)]
    public double Longitude { get; init; }

    [Required]
    public CreateSiteAddressRequest Address { get; init; } = null!;

    [Required]
    public SiteType SiteType { get; init; }

    [StringLength(200)]
    public string BSCName { get; init; } = string.Empty;

    [StringLength(50)]
    public string BSCCode { get; init; } = string.Empty;
}

public record CreateSiteAddressRequest
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

