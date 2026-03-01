namespace TowerOps.Application.DTOs.Visits;

using System;
using System.Collections.Generic;
using TowerOps.Domain.Enums;

public record VisitDto
{
    public Guid Id { get; init; }
    public string VisitNumber { get; init; } = string.Empty;
    public Guid SiteId { get; init; }
    public string SiteCode { get; init; } = string.Empty;
    public string SiteName { get; init; } = string.Empty;
    public Guid EngineerId { get; init; }
    public string EngineerName { get; init; } = string.Empty;
    public string? SupervisorName { get; init; }
    public List<string> TechnicianNames { get; init; } = new();
    public DateTime ScheduledDate { get; init; }
    public DateTime? ActualStartTime { get; init; }
    public DateTime? ActualEndTime { get; init; }
    public DateTime? EngineerReportedCompletionTimeUtc { get; init; }
    public string? Duration { get; init; }
    public VisitStatus Status { get; init; }
    public VisitType Type { get; init; }
    public int CompletionPercentage { get; init; }
    public bool CanBeEdited { get; init; }
    public bool CanBeSubmitted { get; init; }
    public string? EngineerNotes { get; init; }
    public string? ReviewerNotes { get; init; }
    public DateTime CreatedAt { get; init; }
}
