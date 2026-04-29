using Shoply.WebApi.Common.Infrastructure.Services.Pagination;
using Shoply.WebApi.Features.Products.Endpoints.GetProducts;

namespace Shoply.WebApi.Features.Products.Endpoints.SearchProducts;

public sealed class SearchProductQueryHandler(
    IMapper<Product, ProductsResponse> mapper,
    PaginationService paginationService) : IQueryHandler<SearchProductQuery, PaginationResult<ProductsResponse>>
{
    public async ValueTask<PaginationResult<ProductsResponse>> Handle(
        SearchProductQuery query,
        CancellationToken cancellationToken)
    {
        return await paginationService.GetPaginationResultAsync(
            query,
            mapper,
            q => q.Where(p => EF.Functions.ILike(p.Name, $"%{query.Name}%")),
            cancellationToken);
    }
}