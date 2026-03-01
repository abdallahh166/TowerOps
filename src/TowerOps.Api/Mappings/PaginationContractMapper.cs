using TowerOps.Api.Contracts.Common;
using TowerOps.Application.Common;

namespace TowerOps.Api.Mappings;

public static class PaginationContractMapper
{
    public static PagedResponse<T> ToPagedResponse<T>(this PaginatedList<T> source)
    {
        return new PagedResponse<T>
        {
            Data = source.Items,
            Pagination = new PaginationMetadata
            {
                Page = source.PageNumber,
                PageSize = source.PageSize,
                Total = source.TotalCount,
                TotalPages = source.TotalPages,
                HasNextPage = source.HasNextPage,
                HasPreviousPage = source.HasPreviousPage
            }
        };
    }

    public static PagedResponse<T> ToPagedResponse<T>(
        this IReadOnlyList<T> data,
        int page,
        int pageSize,
        int total)
    {
        var normalizedPage = page < 1 ? 1 : page;
        var normalizedSize = pageSize < 1 ? 1 : pageSize;
        var totalPages = total <= 0 ? 0 : (int)Math.Ceiling(total / (double)normalizedSize);

        return new PagedResponse<T>
        {
            Data = data,
            Pagination = new PaginationMetadata
            {
                Page = normalizedPage,
                PageSize = normalizedSize,
                Total = total,
                TotalPages = totalPages,
                HasNextPage = normalizedPage < totalPages,
                HasPreviousPage = normalizedPage > 1
            }
        };
    }
}
