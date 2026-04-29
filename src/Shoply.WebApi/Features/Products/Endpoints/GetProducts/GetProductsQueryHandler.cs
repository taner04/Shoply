using Shoply.WebApi.Common.Infrastructure.Services.Pagination;

namespace Shoply.WebApi.Features.Products.Endpoints.GetProducts;

public sealed class GetProductsQueryHandler(
    PaginationService paginationService,
    IMapper<Product, ProductsResponse> mapper)
    : IQueryHandler<GetProductsQuery, PaginationResult<ProductsResponse>>
{
    public async ValueTask<PaginationResult<ProductsResponse>> Handle(
        GetProductsQuery query,
        CancellationToken cancellationToken) =>
        await paginationService.GetPaginationResultAsync(query, mapper, cancellationToken);
}