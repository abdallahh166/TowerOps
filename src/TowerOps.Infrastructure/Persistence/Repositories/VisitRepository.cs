namespace TowerOps.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using TowerOps.Domain.Entities.Visits;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Interfaces.Repositories;

public class VisitRepository : Repository<Visit, Guid>, IVisitRepository
{
    public VisitRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<Visit?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(v => v.Photos)
            .Include(v => v.Readings)
            .Include(v => v.Checklists)
            .Include(v => v.MaterialsUsed)
            .Include(v => v.IssuesFound)
            .Include(v => v.ApprovalHistory)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
    }

    // ✅ WITH TRACKING - For updates
    public async Task<Visit?> GetByVisitNumberAsync(
        string visitNumber,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(v => v.Photos)
            .Include(v => v.Readings)
            .Include(v => v.Checklists)
            .Include(v => v.MaterialsUsed)
            .Include(v => v.IssuesFound)
            .Include(v => v.ApprovalHistory)
            .FirstOrDefaultAsync(v => v.VisitNumber == visitNumber, cancellationToken);
    }

    public async Task<IReadOnlyList<Visit>> GetBySiteIdAsync(
        Guid siteId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(v => v.SiteId == siteId)
            .OrderByDescending(v => v.ScheduledDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Visit>> GetByEngineerIdAsync(
        Guid engineerId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(v => v.EngineerId == engineerId)
            .OrderByDescending(v => v.ScheduledDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Visit>> GetByStatusAsync(
        VisitStatus status,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(v => v.Status == status)
            .OrderBy(v => v.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Visit>> GetPendingReviewAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(v => v.Status == VisitStatus.Submitted || v.Status == VisitStatus.UnderReview)
            .OrderBy(v => v.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Visit>> GetScheduledVisitsAsync(
        DateTime date,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(v => v.ScheduledDate.Date == date.Date &&
                       (v.Status == VisitStatus.Scheduled || v.Status == VisitStatus.InProgress))
            .OrderBy(v => v.ScheduledDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Visit?> GetActiveVisitForEngineerAsync(
        Guid engineerId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(
                v => v.EngineerId == engineerId && v.Status == VisitStatus.InProgress,
                cancellationToken);
    }

    // ✅ WITHOUT TRACKING - For display/reports
    public async Task<Visit?> GetByVisitNumberAsNoTrackingAsync(
        string visitNumber,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(v => v.Photos)
            .Include(v => v.Readings)
            .Include(v => v.Checklists)
            .Include(v => v.MaterialsUsed)
            .Include(v => v.IssuesFound)
            .Include(v => v.ApprovalHistory)
            .FirstOrDefaultAsync(v => v.VisitNumber == visitNumber, cancellationToken);
    }

    public async Task<IReadOnlyList<Visit>> GetBySiteIdAsNoTrackingAsync(
        Guid siteId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(v => v.SiteId == siteId)
            .OrderByDescending(v => v.ScheduledDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Visit>> GetByEngineerIdAsNoTrackingAsync(
        Guid engineerId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(v => v.EngineerId == engineerId)
            .OrderByDescending(v => v.ScheduledDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Visit>> GetByStatusAsNoTrackingAsync(
        VisitStatus status,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(v => v.Status == status)
            .OrderBy(v => v.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Visit>> GetPendingReviewAsNoTrackingAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(v => v.Status == VisitStatus.Submitted || v.Status == VisitStatus.UnderReview)
            .OrderBy(v => v.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Visit>> GetScheduledVisitsAsNoTrackingAsync(
        DateTime date,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(v => v.ScheduledDate.Date == date.Date &&
                       (v.Status == VisitStatus.Scheduled || v.Status == VisitStatus.InProgress))
            .OrderBy(v => v.ScheduledDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Visit?> GetActiveVisitForEngineerAsNoTrackingAsync(
        Guid engineerId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(
                v => v.EngineerId == engineerId && v.Status == VisitStatus.InProgress,
                cancellationToken);
    }

    // ✅ QUERY METHODS - Always AsNoTracking
    public async Task<string> GenerateVisitNumberAsync(CancellationToken cancellationToken = default)
    {
        var year = DateTime.UtcNow.Year;
        var prefix = $"V{year}";

        var lastVisit = await _dbSet
            .AsNoTracking()
            .Where(v => v.VisitNumber.StartsWith(prefix))
            .OrderByDescending(v => v.VisitNumber)
            .FirstOrDefaultAsync(cancellationToken);

        if (lastVisit == null)
        {
            return $"{prefix}000001";
        }

        var lastNumber = int.Parse(lastVisit.VisitNumber.Substring(5));
        var nextNumber = lastNumber + 1;

        return $"{prefix}{nextNumber:D6}";
    }

    public async Task<bool> VisitNumberExistsAsync(
        string visitNumber,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .AnyAsync(v => v.VisitNumber == visitNumber, cancellationToken);
    }

    public async Task<int> GetVisitCountBySiteAsync(
        Guid siteId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .CountAsync(v => v.SiteId == siteId, cancellationToken);
    }

    public async Task<int> GetVisitCountByEngineerAsync(
        Guid engineerId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .CountAsync(v => v.EngineerId == engineerId, cancellationToken);
    }

    public async Task<int> GetVisitCountByStatusAsync(
        VisitStatus status,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .CountAsync(v => v.Status == status, cancellationToken);
    }

    public async Task<bool> HasActiveVisitAsync(
        Guid engineerId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .AnyAsync(v => v.EngineerId == engineerId && v.Status == VisitStatus.InProgress,
                cancellationToken);
    }
}