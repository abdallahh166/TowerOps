namespace TelecomPm.Api.Contracts.Visits;

using System.ComponentModel.DataAnnotations;
using TelecomPM.Domain.Enums;

public record UpdateChecklistItemRequest
{
    [Required]
    public CheckStatus Status { get; init; }

    [MaxLength(1000)]
    public string? Notes { get; init; }
}
