using TowerOps.Application.Common.Interfaces;
using TowerOps.Domain.Entities.SystemSettings;
using TowerOps.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace TowerOps.Infrastructure.Services;

public sealed class SystemSettingsService : ISystemSettingsService
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);
    private readonly ISystemSettingsRepository _settingsRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISettingsEncryptionService _encryptionService;
    private readonly IMemoryCache _memoryCache;

    public SystemSettingsService(
        ISystemSettingsRepository settingsRepository,
        IUnitOfWork unitOfWork,
        ISettingsEncryptionService encryptionService,
        IMemoryCache memoryCache)
    {
        _settingsRepository = settingsRepository;
        _unitOfWork = unitOfWork;
        _encryptionService = encryptionService;
        _memoryCache = memoryCache;
    }

    public async Task<T> GetAsync<T>(string key, T defaultValue, CancellationToken cancellationToken = default)
    {
        var cacheKey = BuildCacheKey(key);
        var setting = await _memoryCache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CacheDuration;
            return await _settingsRepository.GetAsync(key, cancellationToken);
        });

        if (setting is null)
        {
            return defaultValue;
        }

        var raw = setting.IsEncrypted
            ? _encryptionService.Decrypt(setting.Value)
            : setting.Value;

        return ConvertValue(raw, defaultValue);
    }

    public async Task SetAsync(string key, string value, string updatedBy, CancellationToken cancellationToken = default)
    {
        var existing = await _settingsRepository.GetAsync(key, cancellationToken);

        var group = existing?.Group ?? ResolveGroupFromKey(key);
        var dataType = existing?.DataType ?? ResolveDataType(key, value);
        var isEncrypted = existing?.IsEncrypted ?? dataType.Equals("secret", StringComparison.OrdinalIgnoreCase);

        var persistedValue = isEncrypted
            ? _encryptionService.Encrypt(value)
            : value;

        var setting = SystemSetting.Create(
            key,
            persistedValue,
            group,
            dataType,
            existing?.Description,
            isEncrypted,
            updatedBy);

        await _settingsRepository.UpsertAsync(setting, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _memoryCache.Remove(BuildCacheKey(key));
        _memoryCache.Remove(BuildGroupCacheKey(group));
    }

    public async Task<Dictionary<string, string>> GetGroupAsync(string group, CancellationToken cancellationToken = default)
    {
        var normalizedGroup = string.IsNullOrWhiteSpace(group) ? "General" : group.Trim();
        var settings = await _memoryCache.GetOrCreateAsync(BuildGroupCacheKey(normalizedGroup), async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CacheDuration;
            return await _settingsRepository.GetByGroupAsync(normalizedGroup, cancellationToken);
        }) ?? Array.Empty<SystemSetting>();

        return settings.ToDictionary(
            setting => setting.Key,
            setting => setting.IsEncrypted
                ? _encryptionService.Decrypt(setting.Value)
                : setting.Value,
            StringComparer.OrdinalIgnoreCase);
    }

    private static T ConvertValue<T>(string raw, T defaultValue)
    {
        try
        {
            var targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

            if (targetType == typeof(string))
            {
                return (T)(object)raw;
            }

            if (targetType == typeof(int) && int.TryParse(raw, out var intValue))
            {
                return (T)(object)intValue;
            }

            if (targetType == typeof(bool))
            {
                if (bool.TryParse(raw, out var boolValue))
                    return (T)(object)boolValue;

                if (raw == "1")
                    return (T)(object)true;

                if (raw == "0")
                    return (T)(object)false;
            }

            if (targetType == typeof(decimal) && decimal.TryParse(raw, out var decimalValue))
            {
                return (T)(object)decimalValue;
            }

            return (T)Convert.ChangeType(raw, targetType);
        }
        catch
        {
            return defaultValue;
        }
    }

    private static string ResolveGroupFromKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return "General";

        var parts = key.Split(':', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > 0 ? parts[0] : "General";
    }

    private static string ResolveDataType(string key, string value)
    {
        if (IsSecretKey(key))
            return "secret";

        if (int.TryParse(value, out _))
            return "int";

        if (bool.TryParse(value, out _))
            return "bool";

        return "string";
    }

    private static bool IsSecretKey(string key)
    {
        return key.Contains("password", StringComparison.OrdinalIgnoreCase)
               || key.Contains("token", StringComparison.OrdinalIgnoreCase)
               || key.Contains("secret", StringComparison.OrdinalIgnoreCase)
               || key.Contains("key", StringComparison.OrdinalIgnoreCase)
               || key.Contains("auth", StringComparison.OrdinalIgnoreCase);
    }

    private static string BuildCacheKey(string key) => $"system-setting:{key}";

    private static string BuildGroupCacheKey(string group) => $"system-setting-group:{group}";
}
