namespace TelecomPM.Application.DTOs.Users;

using System;
using TelecomPM.Domain.Enums;

public record UserPerformanceDto
{
    public Guid UserId { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string UserEmail { get; init; } = string.Empty;
    public UserRole Role { get; init; }
    public int AssignedSitesCount { get; init; }
    public int? MaxAssignedSites { get; init; }
    public int TotalVisits { get; init; }
    public int CompletedVisits { get; init; }
    public int ApprovedVisits { get; init; }
    public int RejectedVisits { get; init; }
    public int OnTimeVisits { get; init; }
    public decimal CompletionRate { get; init; }
    public decimal ApprovalRate { get; init; }
    public decimal OnTimeRate { get; init; }
    public decimal? PerformanceRating { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
}

