namespace TelecomPm.Api.Contracts.Roles;

public sealed class CreateRoleRequest
{
    public string Name { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsActive { get; init; } = true;
    public List<string> Permissions { get; init; } = new();
}
