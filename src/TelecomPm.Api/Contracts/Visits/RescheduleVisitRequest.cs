namespace TelecomPm.Api.Contracts.Visits;

using System;
using System.ComponentModel.DataAnnotations;

public record RescheduleVisitRequest
{
    [Required]
    public DateTime NewScheduledDate { get; init; }

    [MaxLength(500)]
    public string? Reason { get; init; }
}
