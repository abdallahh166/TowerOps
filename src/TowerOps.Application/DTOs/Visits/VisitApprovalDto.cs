namespace TowerOps.Application.DTOs.Visits;

using System;
using TowerOps.Domain.Enums;

public record VisitApprovalDto
{
    public Guid Id { get; init; }
    public string ReviewerName { get; init; } = string.Empty;
    public ApprovalAction Action { get; init; }
    public string? Comments { get; init; }
    public DateTime ReviewedAt { get; init; }
}