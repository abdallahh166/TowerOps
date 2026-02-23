using TelecomPM.Application.Common;

namespace TelecomPM.Application.Commands.Settings.UpsertSystemSettings;

public sealed record UpsertSystemSettingsCommand : ICommand
{
    public IReadOnlyList<UpsertSystemSettingItem> Settings { get; init; } = Array.Empty<UpsertSystemSettingItem>();
    public string UpdatedBy { get; init; } = "System";
}

public sealed record UpsertSystemSettingItem
{
    public string Key { get; init; } = string.Empty;
    public string Value { get; init; } = string.Empty;
    public string Group { get; init; } = string.Empty;
    public string DataType { get; init; } = "string";
    public string? Description { get; init; }
    public bool IsEncrypted { get; init; }
}
