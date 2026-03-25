using Api.Common.Shared.Pagination;

namespace Api.Features.Products.Endpoints.GetProducts;

public sealed class GetProductsQueryHandler(PaginationService paginationService, GetProductsMapper mapper)
    : IQueryHandler<GetProductsQuery, PaginationResult<GetProductsResponse>>
{
    public async ValueTask<PaginationResult<GetProductsResponse>> Handle(GetProductsQuery query,
        CancellationToken cancellationToken)
    {
        return await paginationService.GetPaginationResultAsync(query, mapper, cancellationToken);
    }
}