using ProductId = Api.Features.Products.Models.ProductId;

namespace Api.Features.Products.Endpoints.GetProductDetails;

public sealed class GetProductDetailsQueryHandler(ApplicationDbContext context)
    : IQueryHandler<GetProductDetailsQuery, GetProductDetailsResponse>
{
    public async ValueTask<GetProductDetailsResponse> Handle(GetProductDetailsQuery query,
        CancellationToken cancellationToken)
    {
        var product = await context.ProductsQuery
                          .AsNoTracking()
                          .FirstOrDefaultAsync(p => p.Id == ProductId.From(query.ProductId), cancellationToken)
                      ?? throw new EntityNotFoundException<Product>(query.ProductId);

        return GetProductDetailsResponse.FromProduct(product);
    }
}
