namespace TelecomPm.Api.Contracts.Visits;

using System.ComponentModel.DataAnnotations;

public record AddVisitReadingRequest
{
    [Required]
    public string ReadingType { get; init; } = string.Empty;

    [Required]
    public string Category { get; init; } = string.Empty;

    [Required]
    public decimal Value { get; init; }

    [Required]
    public string Unit { get; init; } = string.Empty;

    public decimal? MinAcceptable { get; init; }

    public decimal? MaxAcceptable { get; init; }

    public string? Phase { get; init; }

    public string? Equipment { get; init; }

    [MaxLength(1000)]
    public string? Notes { get; init; }
}

