namespace TowerOps.Domain.Interfaces.Repositories;

using TowerOps.Domain.Common;
using TowerOps.Domain.Specifications;

public interface IRepository<TEntity, TId>
    where TEntity : AggregateRoot<TId>
    where TId : notnull
{
    // ==================== READ OPERATIONS (WITH TRACKING) ====================
    // Use these when you plan to UPDATE the entity after retrieving it

    /// <summary>
    /// Gets an entity by ID with change tracking enabled.
    /// Use this when you plan to modify the entity.
    /// </summary>
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all entities with change tracking enabled.
    /// Use this when you plan to modify the entities.
    /// </summary>
    Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds entities matching a specification with change tracking enabled.
    /// Use this when you plan to modify the entities.
    /// </summary>
    Task<IReadOnlyList<TEntity>> FindAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds a single entity matching a specification with change tracking enabled.
    /// Use this when you plan to modify the entity.
    /// </summary>
    Task<TEntity?> FindOneAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    // ==================== READ OPERATIONS (WITHOUT TRACKING) ====================
    // Use these for read-only operations (display, reports, DTOs)

    /// <summary>
    /// Gets an entity by ID without change tracking.
    /// Use this for read-only operations (display, reports, DTOs).
    /// 30-50% faster than tracked queries.
    /// </summary>
    Task<TEntity?> GetByIdAsNoTrackingAsync(TId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all entities without change tracking.
    /// Use this for read-only operations (display, reports, DTOs).
    /// </summary>
    Task<IReadOnlyList<TEntity>> GetAllAsNoTrackingAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds entities matching a specification without change tracking.
    /// Use this for read-only operations (display, reports, DTOs).
    /// </summary>
    Task<IReadOnlyList<TEntity>> FindAsNoTrackingAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds a single entity matching a specification without change tracking.
    /// Use this for read-only operations (display, reports, DTOs).
    /// </summary>
    Task<TEntity?> FindOneAsNoTrackingAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    // ==================== QUERY OPERATIONS ====================
    // Always use AsNoTracking for performance (just counting/checking existence)

    /// <summary>
    /// Counts entities matching a specification.
    /// Always uses AsNoTracking for optimal performance.
    /// </summary>
    Task<int> CountAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if any entities exist matching a specification.
    /// Always uses AsNoTracking for optimal performance.
    /// </summary>
    Task<bool> ExistsAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    // ==================== WRITE OPERATIONS ====================

    /// <summary>
    /// Adds a new entity to the context.
    /// Call SaveChangesAsync on UnitOfWork to persist.
    /// </summary>
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds multiple new entities to the context.
    /// Call SaveChangesAsync on UnitOfWork to persist.
    /// </summary>
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks an entity as modified.
    /// Call SaveChangesAsync on UnitOfWork to persist.
    /// </summary>
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Soft deletes an entity (marks as deleted).
    /// Call SaveChangesAsync on UnitOfWork to persist.
    /// </summary>
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Soft deletes multiple entities (marks as deleted).
    /// Call SaveChangesAsync on UnitOfWork to persist.
    /// </summary>
    Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
}