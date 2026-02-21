using TelecomPM.Domain.Entities.SystemSettings;

namespace TelecomPM.Domain.Interfaces.Repositories;

public interface ISystemSettingsRepository : IRepository<SystemSetting, Guid>
{
    Task<SystemSetting?> GetAsync(string key, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SystemSetting>> GetByGroupAsync(string group, CancellationToken cancellationToken = default);
    Task UpsertAsync(SystemSetting setting, CancellationToken cancellationToken = default);
    Task UpsertManyAsync(IReadOnlyList<SystemSetting> settings, CancellationToken cancellationToken = default);
}
