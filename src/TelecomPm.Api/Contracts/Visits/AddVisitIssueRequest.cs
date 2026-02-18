namespace TelecomPm.Api.Contracts.Visits;

using System.ComponentModel.DataAnnotations;
using TelecomPM.Domain.Enums;

public record AddVisitIssueRequest
{
    [Required]
    public IssueCategory Category { get; init; }

    [Required]
    public IssueSeverity Severity { get; init; }

    [Required]
    [MaxLength(200)]
    public string Title { get; init; } = string.Empty;

    [Required]
    [MaxLength(2000)]
    public string Description { get; init; } = string.Empty;

    public List<Guid>? PhotoIds { get; init; }
}
