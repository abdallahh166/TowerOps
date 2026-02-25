namespace TowerOps.Api.Contracts.Settings;

public sealed class SystemSettingResponse
{
    public string Key { get; init; } = string.Empty;
    public string Value { get; init; } = string.Empty;
    public string Group { get; init; } = string.Empty;
    public string DataType { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsEncrypted { get; init; }
    public DateTime UpdatedAtUtc { get; init; }
    public string UpdatedBy { get; init; } = string.Empty;
}
