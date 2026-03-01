namespace TowerOps.Api.Contracts.Visits;

using System.ComponentModel.DataAnnotations;

public record CompleteVisitRequest
{
    [MaxLength(2000)]
    public string? EngineerNotes { get; init; }
    public DateTime? EngineerReportedCompletionTimeUtc { get; init; }
}

