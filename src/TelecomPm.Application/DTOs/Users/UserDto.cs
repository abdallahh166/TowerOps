namespace TelecomPM.Application.DTOs.Users;

using System;
using TelecomPM.Domain.Enums;

public record UserDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public UserRole Role { get; init; }
    public Guid OfficeId { get; init; }
    public string OfficeName { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public int? AssignedSitesCount { get; init; }
    public int? MaxAssignedSites { get; init; }
    public decimal? PerformanceRating { get; init; }
}