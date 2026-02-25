using TowerOps.Domain.Entities.Visits;
using TowerOps.Domain.Enums;

namespace TowerOps.Domain.Interfaces.Repositories;

public interface IVisitRepository : IRepository<Visit, Guid>
{
    // ✅ WITH TRACKING - For updates
    Task<Visit?> GetByVisitNumberAsync(string visitNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Visit>> GetBySiteIdAsync(Guid siteId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Visit>> GetByEngineerIdAsync(Guid engineerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Visit>> GetByStatusAsync(VisitStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Visit>> GetPendingReviewAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Visit>> GetScheduledVisitsAsync(DateTime date, CancellationToken cancellationToken = default);
    Task<Visit?> GetActiveVisitForEngineerAsync(Guid engineerId, CancellationToken cancellationToken = default);

    // ✅ WITHOUT TRACKING - For display/reports
    Task<Visit?> GetByVisitNumberAsNoTrackingAsync(string visitNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Visit>> GetBySiteIdAsNoTrackingAsync(Guid siteId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Visit>> GetByEngineerIdAsNoTrackingAsync(Guid engineerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Visit>> GetByStatusAsNoTrackingAsync(VisitStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Visit>> GetPendingReviewAsNoTrackingAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Visit>> GetScheduledVisitsAsNoTrackingAsync(DateTime date, CancellationToken cancellationToken = default);
    Task<Visit?> GetActiveVisitForEngineerAsNoTrackingAsync(Guid engineerId, CancellationToken cancellationToken = default);

    // ✅ QUERY METHODS - Always optimized
    Task<string> GenerateVisitNumberAsync(CancellationToken cancellationToken = default);
    Task<bool> VisitNumberExistsAsync(string visitNumber, CancellationToken cancellationToken = default);
    Task<int> GetVisitCountBySiteAsync(Guid siteId, CancellationToken cancellationToken = default);
    Task<int> GetVisitCountByEngineerAsync(Guid engineerId, CancellationToken cancellationToken = default);
    Task<int> GetVisitCountByStatusAsync(VisitStatus status, CancellationToken cancellationToken = default);
    Task<bool> HasActiveVisitAsync(Guid engineerId, CancellationToken cancellationToken = default);
}