namespace TelecomPm.Api.Contracts.Visits;

using System.ComponentModel.DataAnnotations;

public record CancelVisitRequest
{
    [Required]
    [MaxLength(500)]
    public string Reason { get; init; } = string.Empty;
}
