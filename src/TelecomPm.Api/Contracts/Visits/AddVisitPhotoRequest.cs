namespace TelecomPm.Api.Contracts.Visits;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using TelecomPM.Domain.Enums;

public record AddVisitPhotoRequest
{
    [Required]
    public PhotoType Type { get; init; }

    [Required]
    public PhotoCategory Category { get; init; }

    [Required]
    [MaxLength(256)]
    public string ItemName { get; init; } = string.Empty;

    [Required]
    public IFormFile File { get; init; } = null!;

    [MaxLength(500)]
    public string? Description { get; init; }

    public double? Latitude { get; init; }

    public double? Longitude { get; init; }
}

