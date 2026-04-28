using System.Net;

namespace Shoply.WebApi.Common.Infrastructure.Services.Pagination.Exceptions;

public sealed class PaginationQueryException : ShoplyException
{
    private PaginationQueryException(string title, string message, string errorCode) : base(title, message, errorCode,
        HttpStatusCode.BadRequest)
    {
    }

    public static void ThrowIfInvalidPaginationQuery(PaginationQuery paginationQuery)
    {
        if (paginationQuery is { PageIndex: < 1, PageSize: > PaginationService.MaxPageSize or < 1 })
        {
            throw new PaginationQueryException(
                "Invalid pagination query",
                $"Page index must be at least 1 and page size must be between 1 and {PaginationService.MaxPageSize}",
                "InvalidPaginationQuery");
        }
    }
}