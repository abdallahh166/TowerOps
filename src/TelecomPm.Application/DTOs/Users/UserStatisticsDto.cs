namespace TelecomPM.Application.DTOs.Users;

using System;
using TelecomPM.Domain.Enums;

public record UserStatisticsDto
{
    public Guid UserId { get; init; }
    public string UserName { get; init; } = string.Empty;
    public UserRole Role { get; init; }
    public int TotalSitesAssigned { get; init; }
    public int ActiveVisits { get; init; }
    public int CompletedVisitsThisMonth { get; init; }
    public decimal AveragePerformanceRating { get; init; }
    public DateTime? LastVisitDate { get; init; }
    public bool IsCurrentlyActive { get; init; }
}

