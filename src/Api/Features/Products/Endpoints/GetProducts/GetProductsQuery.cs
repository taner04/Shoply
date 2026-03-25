using Api.Common.Shared.Pagination;

namespace Api.Features.Products.Endpoints.GetProducts;

public sealed record GetProductsQuery(int PageIndex, int PageSize)
    : PaginationQuery(PageIndex, PageSize), IQuery<PaginationResult<GetProductsResponse>>;