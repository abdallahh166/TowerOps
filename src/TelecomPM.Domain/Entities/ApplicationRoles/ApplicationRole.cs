using TelecomPM.Domain.Common;
using TelecomPM.Domain.Exceptions;

namespace TelecomPM.Domain.Entities.ApplicationRoles;

public sealed class ApplicationRole : AggregateRoot<string>
{
    public string Name { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsSystem { get; private set; }
    public bool IsActive { get; private set; }

    private readonly List<string> _permissions = new();
    public IReadOnlyCollection<string> Permissions => _permissions.AsReadOnly();

    private ApplicationRole() : base()
    {
    }

    private ApplicationRole(
        string name,
        string displayName,
        string? description,
        bool isSystem,
        bool isActive,
        IReadOnlyCollection<string> permissions) : base(name)
    {
        Name = name;
        DisplayName = displayName;
        Description = description;
        IsSystem = isSystem;
        IsActive = isActive;
        SetPermissions(permissions);
    }

    public static ApplicationRole Create(
        string name,
        string displayName,
        string? description,
        bool isSystem,
        bool isActive,
        IReadOnlyCollection<string> permissions)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Role name is required.");

        if (string.IsNullOrWhiteSpace(displayName))
            throw new DomainException("Display name is required.");

        var normalizedName = name.Trim();
        return new ApplicationRole(
            normalizedName,
            displayName.Trim(),
            description,
            isSystem,
            isActive,
            permissions);
    }

    public void Update(
        string displayName,
        string? description,
        bool isActive,
        IReadOnlyCollection<string> permissions)
    {
        if (string.IsNullOrWhiteSpace(displayName))
            throw new DomainException("Display name is required.");

        DisplayName = displayName.Trim();
        Description = description;
        IsActive = isActive;
        SetPermissions(permissions);
        MarkAsUpdated("System");
    }

    public void SetPermissions(IReadOnlyCollection<string> permissions)
    {
        _permissions.Clear();
        if (permissions is null || permissions.Count == 0)
            return;

        foreach (var permission in permissions
                     .Where(p => !string.IsNullOrWhiteSpace(p))
                     .Select(p => p.Trim())
                     .Distinct(StringComparer.OrdinalIgnoreCase))
        {
            _permissions.Add(permission);
        }
    }

    public bool CanBeDeleted() => !IsSystem;
}
