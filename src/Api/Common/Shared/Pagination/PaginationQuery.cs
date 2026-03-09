namespace Api.Common.Shared.Pagination;

public abstract record PaginationQuery(int PageIndex, int PageSize);