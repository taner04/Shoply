using Shoply.WebApi.Common.Shared.Pagination;

namespace Shoply.WebApi.Features.Products.Endpoints.GetProducts;

public sealed record GetProductsQuery(int PageIndex, int PageSize)
    : PaginationQuery(PageIndex, PageSize), IQuery<PaginationResult<GetProductsResponse>>;