namespace TowerOps.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using TowerOps.Domain.Entities.SystemSettings;
using TowerOps.Domain.Interfaces.Repositories;

public sealed class SystemSettingsRepository : Repository<SystemSetting, Guid>, ISystemSettingsRepository
{
    public SystemSettingsRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<SystemSetting?> GetAsync(string key, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(
            s => s.Key == key,
            cancellationToken);
    }

    public async Task<IReadOnlyList<SystemSetting>> GetByGroupAsync(string group, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.Group == group)
            .OrderBy(s => s.Key)
            .ToListAsync(cancellationToken);
    }

    public async Task UpsertAsync(SystemSetting setting, CancellationToken cancellationToken = default)
    {
        var existing = await _dbSet.FirstOrDefaultAsync(s => s.Key == setting.Key, cancellationToken);
        if (existing is null)
        {
            await _dbSet.AddAsync(setting, cancellationToken);
            return;
        }

        existing.Update(
            setting.Value,
            setting.Group,
            setting.DataType,
            setting.Description,
            setting.IsEncrypted,
            setting.UpdatedBy);
    }

    public async Task UpsertManyAsync(IReadOnlyList<SystemSetting> settings, CancellationToken cancellationToken = default)
    {
        if (settings.Count == 0)
            return;

        var keys = settings.Select(s => s.Key).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        var existing = await _dbSet
            .Where(s => keys.Contains(s.Key))
            .ToDictionaryAsync(s => s.Key, StringComparer.OrdinalIgnoreCase, cancellationToken);

        foreach (var setting in settings)
        {
            if (!existing.TryGetValue(setting.Key, out var persisted))
            {
                await _dbSet.AddAsync(setting, cancellationToken);
                continue;
            }

            persisted.Update(
                setting.Value,
                setting.Group,
                setting.DataType,
                setting.Description,
                setting.IsEncrypted,
                setting.UpdatedBy);
        }
    }
}
