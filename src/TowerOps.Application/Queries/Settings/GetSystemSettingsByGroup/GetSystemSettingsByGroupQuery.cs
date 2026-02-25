using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Settings;

namespace TowerOps.Application.Queries.Settings.GetSystemSettingsByGroup;

public sealed record GetSystemSettingsByGroupQuery : IQuery<IReadOnlyList<SystemSettingDto>>
{
    public string Group { get; init; } = string.Empty;
    public bool MaskEncryptedValues { get; init; } = true;
}
