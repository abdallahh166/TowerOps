namespace TelecomPm.Api.Contracts.Roles;

public sealed class RoleResponse
{
    public string Name { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsSystem { get; init; }
    public bool IsActive { get; init; }
    public IReadOnlyList<string> Permissions { get; init; } = Array.Empty<string>();
}
