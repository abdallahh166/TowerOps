namespace TelecomPm.Api.Contracts.Visits;

using System;
using System.ComponentModel.DataAnnotations;

public record ApproveVisitRequest
{
    [Required]
    public Guid ReviewerId { get; init; }

    [MaxLength(2000)]
    public string? Notes { get; init; }
}

