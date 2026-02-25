using TowerOps.Domain.Entities.Offices;

namespace TowerOps.Domain.Interfaces.Repositories;

public interface IOfficeRepository : IRepository<Office, Guid>
{
    // ✅ WITH TRACKING - For updates
    Task<Office?> GetByCodeAsync(string officeCode, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Office>> GetByRegionAsync(string region, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Office>> GetActiveOfficesAsync(CancellationToken cancellationToken = default);

    // ✅ WITHOUT TRACKING - For display/reports
    Task<Office?> GetByCodeAsNoTrackingAsync(string officeCode, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Office>> GetByRegionAsNoTrackingAsync(string region, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Office>> GetActiveOfficesAsNoTrackingAsync(CancellationToken cancellationToken = default);

    // ✅ QUERY METHODS - Always optimized
    Task<bool> IsOfficeCodeUniqueAsync(string officeCode, Guid? excludeOfficeId = null, CancellationToken cancellationToken = default);
    Task<bool> CodeExistsAsync(string officeCode, CancellationToken cancellationToken = default);
    Task<int> GetOfficeCountByRegionAsync(string region, CancellationToken cancellationToken = default);
    Task<int> GetActiveOfficeCountAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetAllRegionsAsync(CancellationToken cancellationToken = default);
}