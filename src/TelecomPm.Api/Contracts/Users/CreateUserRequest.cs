namespace TelecomPm.Api.Contracts.Users;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TelecomPM.Domain.Enums;

public record CreateUserRequest
{
    [Required]
    [StringLength(200)]
    public string Name { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(200)]
    public string Email { get; init; } = string.Empty;

    [Required]
    [Phone]
    [StringLength(50)]
    public string PhoneNumber { get; init; } = string.Empty;

    [Required]
    [StringLength(200, MinimumLength = 8)]
    public string Password { get; init; } = string.Empty;

    [Required]
    public UserRole Role { get; init; }

    [Required]
    public Guid OfficeId { get; init; }

    [Range(1, 100)]
    public int? MaxAssignedSites { get; init; }

    public List<string>? Specializations { get; init; }
}