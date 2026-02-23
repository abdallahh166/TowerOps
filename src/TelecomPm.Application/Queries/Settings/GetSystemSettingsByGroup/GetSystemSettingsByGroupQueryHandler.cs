using MediatR;
using TelecomPM.Application.Common;
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Application.DTOs.Settings;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Application.Queries.Settings.GetSystemSettingsByGroup;

public sealed class GetSystemSettingsByGroupQueryHandler : IRequestHandler<GetSystemSettingsByGroupQuery, Result<IReadOnlyList<SystemSettingDto>>>
{
    private readonly ISystemSettingsRepository _settingsRepository;
    private readonly ISettingsEncryptionService _settingsEncryptionService;

    public GetSystemSettingsByGroupQueryHandler(
        ISystemSettingsRepository settingsRepository,
        ISettingsEncryptionService settingsEncryptionService)
    {
        _settingsRepository = settingsRepository;
        _settingsEncryptionService = settingsEncryptionService;
    }

    public async Task<Result<IReadOnlyList<SystemSettingDto>>> Handle(GetSystemSettingsByGroupQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Group))
            return Result.Failure<IReadOnlyList<SystemSettingDto>>("Group is required.");

        var settings = await _settingsRepository.GetByGroupAsync(request.Group.Trim(), cancellationToken);
        var result = settings
            .OrderBy(s => s.Key)
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
