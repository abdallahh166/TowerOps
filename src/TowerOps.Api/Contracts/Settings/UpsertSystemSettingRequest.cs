namespace TowerOps.Api.Contracts.Settings;

public sealed class UpsertSystemSettingRequest
{
    public string Key { get; init; } = string.Empty;
    public string Value { get; init; } = string.Empty;
    public string Group { get; init; } = string.Empty;
    public string DataType { get; init; } = "string";
    public string? Description { get; init; }
    public bool IsEncrypted { get; init; }
}
