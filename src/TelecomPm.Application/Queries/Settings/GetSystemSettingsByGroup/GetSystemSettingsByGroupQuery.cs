using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Settings;

namespace TelecomPM.Application.Queries.Settings.GetSystemSettingsByGroup;

public sealed record GetSystemSettingsByGroupQuery : IQuery<IReadOnlyList<SystemSettingDto>>
{
    public string Group { get; init; } = string.Empty;
    public bool MaskEncryptedValues { get; init; } = true;
}
