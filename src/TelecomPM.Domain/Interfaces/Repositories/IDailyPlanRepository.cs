using TelecomPM.Domain.Entities.DailyPlans;

namespace TelecomPM.Domain.Interfaces.Repositories;

public interface IDailyPlanRepository : IRepository<DailyPlan, Guid>
{
    Task<DailyPlan?> GetByOfficeAndDateAsync(Guid officeId, DateOnly date, CancellationToken cancellationToken = default);
    Task<DailyPlan?> GetByOfficeAndDateAsNoTrackingAsync(Guid officeId, DateOnly date, CancellationToken cancellationToken = default);
}
