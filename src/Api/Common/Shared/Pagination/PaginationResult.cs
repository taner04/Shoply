namespace Api.Common.Shared.Pagination;

public sealed class PaginationResult<T>(List<T> items, int pageIndex, int totalPages, int totalCount)
{
    public List<T> Items { get; } = items;
    public int PageIndex { get; } = pageIndex;
    public int TotalPages { get; } = totalPages;
    public int TotalCount { get; } = totalCount;
    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;
}