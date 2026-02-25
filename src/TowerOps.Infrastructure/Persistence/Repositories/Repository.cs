namespace TowerOps.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using TowerOps.Domain.Common;
using TowerOps.Domain.Interfaces.Repositories;
using TowerOps.Domain.Specifications;

public class Repository<TEntity, TId> : IRepository<TEntity, TId>
    where TEntity : AggregateRoot<TId>
    where TId : notnull
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = context.Set<TEntity>();
    }

    // ✅ WITH TRACKING - For entities that will be modified
    public virtual async Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    public virtual async Task<IReadOnlyList<TEntity>> FindAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(specification).ToListAsync(cancellationToken);
    }

    public virtual async Task<TEntity?> FindOneAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(specification).FirstOrDefaultAsync(cancellationToken);
    }

    // ✅ WITHOUT TRACKING - For display/reporting
    public virtual async Task<TEntity?> GetByIdAsNoTrackingAsync(TId id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id.Equals(id), cancellationToken);
    }

    public virtual async Task<IReadOnlyList<TEntity>> GetAllAsNoTrackingAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking().ToListAsync(cancellationToken);
    }

    public virtual async Task<IReadOnlyList<TEntity>> FindAsNoTrackingAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(specification, asNoTracking: true).ToListAsync(cancellationToken);
    }

    public virtual async Task<TEntity?> FindOneAsNoTrackingAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        return await ApplySpecification(specification, asNoTracking: true).FirstOrDefaultAsync(cancellationToken);
    }

    // ✅ QUERY OPERATIONS - Always AsNoTracking
    public virtual async Task<int> CountAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        // Use optimized count query without includes
        var query = _dbSet.AsNoTracking();

        if (specification.Criteria != null)
        {
            query = query.Where(specification.Criteria);
        }

        return await query.CountAsync(cancellationToken);
    }

    public virtual async Task<bool> ExistsAsync(
        ISpecification<TEntity> specification,
        CancellationToken cancellationToken = default)
    {
        // Use optimized exists query without includes
        var query = _dbSet.AsNoTracking();

        if (specification.Criteria != null)
        {
            query = query.Where(specification.Criteria);
        }

        return await query.AnyAsync(cancellationToken);
    }

    // ✅ WRITE OPERATIONS
    public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public virtual async Task AddRangeAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default)
    {
        await _dbSet.AddRangeAsync(entities, cancellationToken);
    }

    public virtual Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    public virtual Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        // Soft delete
        entity.MarkAsDeleted("System");
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    public virtual Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
        {
            entity.MarkAsDeleted("System");
        }
        _dbSet.UpdateRange(entities);
        return Task.CompletedTask;
    }

    // ✅ HELPER METHODS
    protected IQueryable<TEntity> Query()
    {
        return _dbSet.AsQueryable();
    }

    protected IQueryable<TEntity> QueryAsNoTracking()
    {
        return _dbSet.AsNoTracking();
    }

    // ✅ Enhanced with tracking control
    private IQueryable<TEntity> ApplySpecification(
        ISpecification<TEntity> specification,
        bool asNoTracking = false)
    {
        var query = asNoTracking
            ? _dbSet.AsNoTracking()
            : _dbSet.AsQueryable();

        return SpecificationEvaluator<TEntity, TId>.GetQuery(query, specification, asNoTracking);
    }
}