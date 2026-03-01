namespace TowerOps.Api.Contracts.Common;

public sealed class PagedResponse<T>
{
    public IReadOnlyList<T> Data { get; init; } = Array.Empty<T>();
    public PaginationMetadata Pagination { get; init; } = new();
}

public sealed class PaginationMetadata
{
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int Total { get; init; }
    public int TotalPages { get; init; }
    public bool HasNextPage { get; init; }
    public bool HasPreviousPage { get; init; }
}
