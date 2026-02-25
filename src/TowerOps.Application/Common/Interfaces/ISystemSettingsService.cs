namespace TowerOps.Application.Common.Interfaces;

public interface ISystemSettingsService
{
    Task<T> GetAsync<T>(string key, T defaultValue, CancellationToken cancellationToken = default);
    Task SetAsync(string key, string value, string updatedBy, CancellationToken cancellationToken = default);
    Task<Dictionary<string, string>> GetGroupAsync(string group, CancellationToken cancellationToken = default);
}
