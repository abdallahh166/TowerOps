using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Settings;

namespace TowerOps.Application.Queries.Settings.GetAllSystemSettings;

public sealed record GetAllSystemSettingsQuery : IQuery<IReadOnlyList<SystemSettingDto>>
{
    public bool MaskEncryptedValues { get; init; } = true;
    public int? PageNumber { get; init; }
    public int? PageSize { get; init; }
}
