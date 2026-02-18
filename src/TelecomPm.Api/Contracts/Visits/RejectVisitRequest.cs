namespace TelecomPm.Api.Contracts.Visits;

using System;
using System.ComponentModel.DataAnnotations;

public record RejectVisitRequest
{
    [Required]
    public Guid ReviewerId { get; init; }

    [Required]
    [MaxLength(2000)]
    public string RejectionReason { get; init; } = string.Empty;
}

