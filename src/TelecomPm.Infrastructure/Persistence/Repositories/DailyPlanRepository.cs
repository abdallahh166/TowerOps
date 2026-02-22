using Microsoft.EntityFrameworkCore;
using TelecomPM.Domain.Entities.DailyPlans;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Infrastructure.Persistence.Repositories;

public sealed class DailyPlanRepository : Repository<DailyPlan, Guid>, IDailyPlanRepository
{
    public DailyPlanRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<DailyPlan?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(x => x.EngineerPlans)
            .ThenInclude(x => x.Stops)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public override async Task<DailyPlan?> GetByIdAsNoTrackingAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(x => x.EngineerPlans)
            .ThenInclude(x => x.Stops)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<DailyPlan?> GetByOfficeAndDateAsync(Guid officeId, DateOnly date, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(x => x.EngineerPlans)
            .ThenInclude(x => x.Stops)
            .FirstOrDefaultAsync(x => x.OfficeId == officeId && x.PlanDate == date, cancellationToken);
    }

    public async Task<DailyPlan?> GetByOfficeAndDateAsNoTrackingAsync(Guid officeId, DateOnly date, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(x => x.EngineerPlans)
            .ThenInclude(x => x.Stops)
            .FirstOrDefaultAsync(x => x.OfficeId == officeId && x.PlanDate == date, cancellationToken);
    }
}
