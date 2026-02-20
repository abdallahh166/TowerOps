namespace TelecomPm.Api.Contracts.Visits;

using System.ComponentModel.DataAnnotations;

public record UpdateVisitReadingRequest
{
    [Required]
    public decimal Value { get; init; }
}
