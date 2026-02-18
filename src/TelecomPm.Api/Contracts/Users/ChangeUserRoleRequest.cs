namespace TelecomPm.Api.Contracts.Users;

using System.ComponentModel.DataAnnotations;
using TelecomPM.Domain.Enums;

public record ChangeUserRoleRequest
{
    [Required]
    public UserRole NewRole { get; init; }
}

