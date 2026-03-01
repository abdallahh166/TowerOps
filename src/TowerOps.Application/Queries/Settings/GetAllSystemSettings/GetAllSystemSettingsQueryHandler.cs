using TowerOps.Application.Common;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Application.DTOs.Settings;
using TowerOps.Domain.Interfaces.Repositories;
using TowerOps.Domain.Specifications.SystemSettingSpecifications;
using MediatR;

namespace TowerOps.Application.Queries.Settings.GetAllSystemSettings;

public sealed class GetAllSystemSettingsQueryHandler : IRequestHandler<GetAllSystemSettingsQuery, Result<PaginatedList<SystemSettingDto>>>
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

    public async Task<Result<PaginatedList<SystemSettingDto>>> Handle(GetAllSystemSettingsQuery request, CancellationToken cancellationToken)
    {
        var pageNumber = request.Page < 1 ? 1 : request.Page;
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var sortBy = string.IsNullOrWhiteSpace(request.SortBy) ? "group" : request.SortBy.Trim();

        var specification = new AllSystemSettingsSpecification(
            (pageNumber - 1) * pageSize,
            pageSize,
            sortBy,
            request.SortDescending);
        var totalCount = await _settingsRepository.CountAsync(specification, cancellationToken);
        var settings = await _settingsRepository.FindAsNoTrackingAsync(specification, cancellationToken);

        var result = settings
            .Select(s => MapToDto(s, request.MaskEncryptedValues))
            .ToList();

        var paged = new PaginatedList<SystemSettingDto>(result, totalCount, pageNumber, pageSize);
        return Result.Success(paged);
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
