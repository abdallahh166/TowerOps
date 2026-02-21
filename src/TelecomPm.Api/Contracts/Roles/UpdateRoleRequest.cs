namespace TelecomPm.Api.Contracts.Roles;

public sealed class UpdateRoleRequest
{
    public string DisplayName { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsActive { get; init; } = true;
    public List<string> Permissions { get; init; } = new();
}
