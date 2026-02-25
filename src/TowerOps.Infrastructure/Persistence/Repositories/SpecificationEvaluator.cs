namespace TowerOps.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using TowerOps.Domain.Common;
using TowerOps.Domain.Specifications;

public static class SpecificationEvaluator<TEntity, TId>
    where TEntity : AggregateRoot<TId>
    where TId : notnull
{
    public static IQueryable<TEntity> GetQuery(
        IQueryable<TEntity> inputQuery,
        ISpecification<TEntity> specification,
        bool asNoTracking = false)
    {
        var query = inputQuery;

        // ✅ Apply AsNoTracking if specified
        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        // Apply criteria
        if (specification.Criteria != null)
        {
            query = query.Where(specification.Criteria);
        }

        // Apply includes
        query = specification.Includes
            .Aggregate(query, (current, include) => current.Include(include));

        // Apply string-based includes
        query = specification.IncludeStrings
            .Aggregate(query, (current, include) => current.Include(include));

        // Apply ordering
        if (specification.OrderBy != null)
        {
            query = query.OrderBy(specification.OrderBy);
        }
        else if (specification.OrderByDescending != null)
        {
            query = query.OrderByDescending(specification.OrderByDescending);
        }

        // Apply ThenBy ordering
        if (specification.OrderBy != null || specification.OrderByDescending != null)
        {
            var orderedQuery = (IOrderedQueryable<TEntity>)query;

            foreach (var thenBy in specification.ThenBy)
            {
                orderedQuery = orderedQuery.ThenBy(thenBy);
            }

            foreach (var thenByDesc in specification.ThenByDescending)
            {
                orderedQuery = orderedQuery.ThenByDescending(thenByDesc);
            }

            query = orderedQuery;
        }

        // Apply paging (must be last)
        if (specification.IsPagingEnabled)
        {
            query = query.Skip(specification.Skip).Take(specification.Take);
        }

        return query;
    }

    /// <summary>
    /// Gets query for counting total records without pagination
    /// </summary>
    public static IQueryable<TEntity> GetQueryForCount(
        IQueryable<TEntity> inputQuery,
        ISpecification<TEntity> specification)
    {
        var query = inputQuery.AsNoTracking();

        // Apply only criteria for counting (no includes, ordering, or paging)
        if (specification.Criteria != null)
        {
            query = query.Where(specification.Criteria);
        }

        return query;
    }
}