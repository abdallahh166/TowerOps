namespace TowerOps.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using TowerOps.Domain.Entities.Materials;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Interfaces.Repositories;

public class MaterialRepository : Repository<Material, Guid>, IMaterialRepository
{
    public MaterialRepository(ApplicationDbContext context) : base(context)
    {
    }

    // ✅ WITH TRACKING - For updates
    public async Task<Material?> GetByCodeAsync(
        string materialCode,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(m => m.Code == materialCode, cancellationToken);
    }

    public async Task<IReadOnlyList<Material>> GetByOfficeIdAsync(
        Guid officeId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(m => m.OfficeId == officeId && m.IsActive)
            .OrderBy(m => m.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Material>> GetLowStockItemsAsync(
        Guid officeId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(m => m.OfficeId == officeId &&
                       m.IsActive &&
                       m.CurrentStock.Value <= m.MinimumStock.Value)
            .OrderBy(m => m.CurrentStock.Value)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Material>> GetByCategoryAsync(
        MaterialCategory category,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(m => m.Category == category && m.IsActive)
            .OrderBy(m => m.Name)
            .ToListAsync(cancellationToken);
    }

    // ✅ WITHOUT TRACKING - For display/reports
    public async Task<Material?> GetByCodeAsNoTrackingAsync(
        string materialCode,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Code == materialCode, cancellationToken);
    }

    public async Task<IReadOnlyList<Material>> GetByOfficeIdAsNoTrackingAsync(
        Guid officeId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(m => m.OfficeId == officeId && m.IsActive)
            .OrderBy(m => m.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Material>> GetLowStockItemsAsNoTrackingAsync(
        Guid officeId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(m => m.OfficeId == officeId &&
                       m.IsActive &&
                       m.CurrentStock.Value <= m.MinimumStock.Value)
            .OrderBy(m => m.CurrentStock.Value)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Material>> GetByCategoryAsNoTrackingAsync(
        MaterialCategory category,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(m => m.Category == category && m.IsActive)
            .OrderBy(m => m.Name)
            .ToListAsync(cancellationToken);
    }

    // ✅ QUERY METHODS - Always AsNoTracking
    public async Task<bool> CodeExistsAsync(
        string materialCode,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .AnyAsync(m => m.Code == materialCode, cancellationToken);
    }

    public async Task<int> GetLowStockCountAsync(
        Guid officeId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .CountAsync(m => m.OfficeId == officeId &&
                            m.IsActive &&
                            m.CurrentStock.Value <= m.MinimumStock.Value,
                       cancellationToken);
    }

    // ✅ COMMAND METHOD - Uses tracking (calls GetByIdAsync which tracks)
    public async Task UpdateStockAsync(
        Guid materialId,
        decimal quantity,
        CancellationToken cancellationToken = default)
    {
        var material = await GetByIdAsync(materialId, cancellationToken);
        if (material == null)
            throw new InvalidOperationException($"Material with ID {materialId} not found");

        // Stock update is handled in domain
        await Task.CompletedTask;
    }
}