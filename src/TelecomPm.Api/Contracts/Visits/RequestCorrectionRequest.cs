namespace TelecomPm.Api.Contracts.Visits;

using System.ComponentModel.DataAnnotations;

public record RequestCorrectionRequest
{
    [Required]
    [MaxLength(2000)]
    public string CorrectionNotes { get; init; } = string.Empty;
}

