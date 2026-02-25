using MediatR;
using TowerOps.Application.Common;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Domain.Entities.SystemSettings;
using TowerOps.Domain.Interfaces.Repositories;

namespace TowerOps.Application.Commands.Settings.UpsertSystemSettings;

public sealed class UpsertSystemSettingsCommandHandler : IRequestHandler<UpsertSystemSettingsCommand, Result>
{
    private readonly ISystemSettingsRepository _settingsRepository;
    private readonly ISettingsEncryptionService _settingsEncryptionService;
    private readonly IUnitOfWork _unitOfWork;

    public UpsertSystemSettingsCommandHandler(
        ISystemSettingsRepository settingsRepository,
        ISettingsEncryptionService settingsEncryptionService,
        IUnitOfWork unitOfWork)
    {
        _settingsRepository = settingsRepository;
        _settingsEncryptionService = settingsEncryptionService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpsertSystemSettingsCommand request, CancellationToken cancellationToken)
    {
        if (request.Settings.Count == 0)
            return Result.Failure("At least one setting is required.");

        var settings = request.Settings
            .Where(s => !string.IsNullOrWhiteSpace(s.Key))
            .Select(s => SystemSetting.Create(
                s.Key.Trim(),
                s.IsEncrypted
                    ? _settingsEncryptionService.Encrypt(s.Value ?? string.Empty)
                    : s.Value ?? string.Empty,
                string.IsNullOrWhiteSpace(s.Group) ? ResolveGroupFromKey(s.Key) : s.Group.Trim(),
                string.IsNullOrWhiteSpace(s.DataType) ? "string" : s.DataType.Trim(),
                s.Description,
                s.IsEncrypted,
                string.IsNullOrWhiteSpace(request.UpdatedBy) ? "System" : request.UpdatedBy.Trim()))
            .ToList();

        if (settings.Count == 0)
            return Result.Failure("At least one valid setting key is required.");

        await _settingsRepository.UpsertManyAsync(settings, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private static string ResolveGroupFromKey(string key)
    {
        var parts = key.Split(':', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > 0 ? parts[0] : "General";
    }
}
