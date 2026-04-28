namespace Shoply.WebApi.Common.Infrastructure.Services.Pagination;

public abstract record PaginationQuery(int PageIndex, int PageSize);