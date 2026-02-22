using TelecomPM.Application.Common;
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Application.DTOs.Settings;
using TelecomPM.Domain.Interfaces.Repositories;
using MediatR;

namespace TelecomPM.Application.Queries.Settings.GetAllSystemSettings;

public sealed class GetAllSystemSettingsQueryHandler : IRequestHandler<GetAllSystemSettingsQuery, Result<IReadOnlyList<SystemSettingDto>>>
{
    private readonly ISystemSettingsRepository _settingsRepository;
    private readonly ISettingsEncryptionService _settingsEncryptionService;

    public GetAllSystemSettingsQueryHandler(
        ISystemSettingsRepository settingsRepository,
        ISettingsEncryptionService settingsEncryptionService)
    {
        _settingsRepository = settingsRepository;
        _settingsEncryptionService = settingsEncryptionService;
    }

    public async Task<Result<IReadOnlyList<SystemSettingDto>>> Handle(GetAllSystemSettingsQuery request, CancellationToken cancellationToken)
    {
        var settings = await _settingsRepository.GetAllAsNoTrackingAsync(cancellationToken);

        var result = settings
            .OrderBy(s => s.Group)
            .ThenBy(s => s.Key)
            .Select(s => MapToDto(s, request.MaskEncryptedValues))
            .ToList();

        return Result.Success<IReadOnlyList<SystemSettingDto>>(result);
    }

    private SystemSettingDto MapToDto(Domain.Entities.SystemSettings.SystemSetting setting, bool maskEncryptedValues)
    {
        var value = setting.Value;
        if (setting.IsEncrypted)
        {
            value = maskEncryptedValues ? "***" : _settingsEncryptionService.Decrypt(setting.Value);
        }

        return new SystemSettingDto
        {
            Key = setting.Key,
            Value = value,
            Group = setting.Group,
            DataType = setting.DataType,
            Description = setting.Description,
            IsEncrypted = setting.IsEncrypted,
            UpdatedAtUtc = setting.UpdatedAtUtc,
            UpdatedBy = setting.UpdatedBy
        };
    }
}
