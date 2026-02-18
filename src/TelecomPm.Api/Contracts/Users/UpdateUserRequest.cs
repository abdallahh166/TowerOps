namespace TelecomPm.Api.Contracts.Users;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public record UpdateUserRequest
{
    [Required]
    [StringLength(200)]
    public string Name { get; init; } = string.Empty;

    [Required]
    [Phone]
    [StringLength(50)]
    public string PhoneNumber { get; init; } = string.Empty;

    [Range(1, 100)]
    public int? MaxAssignedSites { get; init; }

    public List<string>? Specializations { get; init; }
}

