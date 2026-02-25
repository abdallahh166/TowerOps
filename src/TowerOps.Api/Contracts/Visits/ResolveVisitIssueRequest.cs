namespace TowerOps.Api.Contracts.Visits;

using System.ComponentModel.DataAnnotations;

public record ResolveVisitIssueRequest
{
    [Required]
    [MaxLength(2000)]
    public string Resolution { get; init; } = string.Empty;
}
