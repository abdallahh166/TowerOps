using TelecomPM.Domain.Entities.Materials;
using TelecomPM.Domain.Enums;

namespace TelecomPM.Domain.Interfaces.Repositories;

public interface IMaterialRepository : IRepository<Material, Guid>
{
    // ✅ WITH TRACKING - For updates
    Task<Material?> GetByCodeAsync(string materialCode, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Material>> GetByOfficeIdAsync(Guid officeId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Material>> GetLowStockItemsAsync(Guid officeId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Material>> GetByCategoryAsync(MaterialCategory category, CancellationToken cancellationToken = default);

    // ✅ WITHOUT TRACKING - For display/reports
    Task<Material?> GetByCodeAsNoTrackingAsync(string materialCode, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Material>> GetByOfficeIdAsNoTrackingAsync(Guid officeId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Material>> GetLowStockItemsAsNoTrackingAsync(Guid officeId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Material>> GetByCategoryAsNoTrackingAsync(MaterialCategory category, CancellationToken cancellationToken = default);

    // ✅ QUERY METHODS - Always optimized
    Task<bool> CodeExistsAsync(string materialCode, CancellationToken cancellationToken = default);
    Task<int> GetLowStockCountAsync(Guid officeId, CancellationToken cancellationToken = default);

    // ✅ COMMAND METHOD - Already uses tracking internally
    Task UpdateStockAsync(Guid materialId, decimal quantity, CancellationToken cancellationToken = default);
}