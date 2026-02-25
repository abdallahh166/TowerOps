using TowerOps.Application.Common;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Application.DTOs.Settings;
using TowerOps.Domain.Interfaces.Repositories;
using TowerOps.Domain.Specifications.SystemSettingSpecifications;
using MediatR;

namespace TowerOps.Application.Queries.Settings.GetAllSystemSettings;

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
        var pageNumber = request.PageNumber.GetValueOrDefault(1);
        if (pageNumber < 1)
        {
            pageNumber = 1;
        }

        var pageSize = request.PageSize.GetValueOrDefault(100);
        if (pageSize < 1)
        {
            pageSize = 1;
        }

        if (pageSize > 200)
        {
            pageSize = 200;
        }

        var specification = new AllSystemSettingsSpecification(
            (pageNumber - 1) * pageSize,
            pageSize);
        var settings = await _settingsRepository.FindAsNoTrackingAsync(specification, cancellationToken);

        var result = settings
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
