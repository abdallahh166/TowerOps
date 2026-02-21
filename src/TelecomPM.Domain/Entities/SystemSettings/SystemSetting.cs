using TelecomPM.Domain.Common;
using TelecomPM.Domain.Exceptions;

namespace TelecomPM.Domain.Entities.SystemSettings;

public sealed class SystemSetting : AggregateRoot<Guid>
{
    public string Key { get; private set; } = string.Empty;
    public string Value { get; private set; } = string.Empty;
    public string Group { get; private set; } = string.Empty;
    public string DataType { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsEncrypted { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }
    public new string UpdatedBy { get; private set; } = string.Empty;

    private SystemSetting() : base()
    {
    }

    private SystemSetting(
        string key,
        string value,
        string group,
        string dataType,
        string? description,
        bool isEncrypted,
        string updatedBy) : base(Guid.NewGuid())
    {
        Key = key;
        Value = value;
        Group = group;
        DataType = dataType;
        Description = description;
        IsEncrypted = isEncrypted;
        UpdatedAtUtc = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public static SystemSetting Create(
        string key,
        string value,
        string group,
        string dataType,
        string? description,
        bool isEncrypted,
        string updatedBy)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new DomainException("Setting key is required.");

        if (string.IsNullOrWhiteSpace(group))
            throw new DomainException("Setting group is required.");

        if (string.IsNullOrWhiteSpace(dataType))
            throw new DomainException("Setting data type is required.");

        if (string.IsNullOrWhiteSpace(updatedBy))
            throw new DomainException("UpdatedBy is required.");

        return new SystemSetting(
            key.Trim(),
            value ?? string.Empty,
            group.Trim(),
            dataType.Trim(),
            description,
            isEncrypted,
            updatedBy.Trim());
    }

    public void Update(
        string value,
        string group,
        string dataType,
        string? description,
        bool isEncrypted,
        string updatedBy)
    {
        if (string.IsNullOrWhiteSpace(group))
            throw new DomainException("Setting group is required.");

        if (string.IsNullOrWhiteSpace(dataType))
            throw new DomainException("Setting data type is required.");

        if (string.IsNullOrWhiteSpace(updatedBy))
            throw new DomainException("UpdatedBy is required.");

        Value = value ?? string.Empty;
        Group = group.Trim();
        DataType = dataType.Trim();
        Description = description;
        IsEncrypted = isEncrypted;
        UpdatedBy = updatedBy.Trim();
        UpdatedAtUtc = DateTime.UtcNow;
        MarkAsUpdated(updatedBy);
    }
}
