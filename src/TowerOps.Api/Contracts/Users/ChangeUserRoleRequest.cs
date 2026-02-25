namespace TowerOps.Api.Contracts.Users;

using System.ComponentModel.DataAnnotations;
using TowerOps.Domain.Enums;

public record ChangeUserRoleRequest
{
    [Required]
    public UserRole NewRole { get; init; }
}

