namespace TelecomPm.Api.Contracts.Visits;

using System;
using System.ComponentModel.DataAnnotations;

public record RequestCorrectionRequest
{
    [Required]
    public Guid ReviewerId { get; init; }

    [Required]
    [MaxLength(2000)]
    public string CorrectionNotes { get; init; } = string.Empty;
}

