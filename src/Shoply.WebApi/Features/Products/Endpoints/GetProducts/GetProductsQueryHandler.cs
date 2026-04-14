using Shoply.WebApi.Common.Shared.Pagination;

namespace Shoply.WebApi.Features.Products.Endpoints.GetProducts;

public sealed class GetProductsQueryHandler(PaginationService paginationService, GetProductsMapper mapper)
    : IQueryHandler<GetProductsQuery, PaginationResult<GetProductsResponse>>
{
    public async ValueTask<PaginationResult<GetProductsResponse>> Handle(
        GetProductsQuery query,
        CancellationToken cancellationToken) =>
        await paginationService.GetPaginationResultAsync(query, mapper, cancellationToken);
}