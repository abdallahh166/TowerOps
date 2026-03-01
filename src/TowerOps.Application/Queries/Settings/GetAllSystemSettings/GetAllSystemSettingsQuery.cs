using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Settings;

namespace TowerOps.Application.Queries.Settings.GetAllSystemSettings;

public sealed record GetAllSystemSettingsQuery : IQuery<PaginatedList<SystemSettingDto>>
{
    public bool MaskEncryptedValues { get; init; } = true;
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 25;
    public string? SortBy { get; init; }
    public bool SortDescending { get; init; } = true;
}
