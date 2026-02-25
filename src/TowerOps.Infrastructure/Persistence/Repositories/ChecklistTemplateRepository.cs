namespace TowerOps.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using TowerOps.Domain.Entities.ChecklistTemplates;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Interfaces.Repositories;

public class ChecklistTemplateRepository : Repository<ChecklistTemplate, Guid>, IChecklistTemplateRepository
{
    public ChecklistTemplateRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<ChecklistTemplate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.Items)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public override async Task<ChecklistTemplate?> GetByIdAsNoTrackingAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(t => t.Items)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<ChecklistTemplate?> GetActiveByVisitTypeAsync(VisitType visitType, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.Items)
            .Where(t => t.VisitType == visitType && t.IsActive)
            .OrderByDescending(t => t.EffectiveFromUtc)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ChecklistTemplate>> GetByVisitTypeAsync(VisitType visitType, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(t => t.Items)
            .Where(t => t.VisitType == visitType)
            .OrderByDescending(t => t.EffectiveFromUtc)
            .ToListAsync(cancellationToken);
    }
}
