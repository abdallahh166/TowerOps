namespace TelecomPm.Api.Contracts.Visits;

using System.ComponentModel.DataAnnotations;

public record RejectVisitRequest
{
    [Required]
    [MaxLength(2000)]
    public string RejectionReason { get; init; } = string.Empty;
}

