namespace TowerOps.Application.DTOs.Visits;

using System;
using System.Collections.Generic;
using TowerOps.Domain.Enums;

public record VisitIssueDto
{
    public Guid Id { get; init; }
    public IssueCategory Category { get; init; }
    public IssueSeverity Severity { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public IssueStatus Status { get; init; }
    public DateTime ReportedAt { get; init; }
    public DateTime? ResolvedAt { get; init; }
    public string? Resolution { get; init; }
    public List<string> PhotoUrls { get; init; } = new();
}