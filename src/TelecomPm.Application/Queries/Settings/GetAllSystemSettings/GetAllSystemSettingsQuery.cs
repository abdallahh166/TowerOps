using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Settings;

namespace TelecomPM.Application.Queries.Settings.GetAllSystemSettings;

public sealed record GetAllSystemSettingsQuery : IQuery<IReadOnlyList<SystemSettingDto>>
{
    public bool MaskEncryptedValues { get; init; } = true;
}
