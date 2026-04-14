namespace Shoply.WebApi.Features.Products.Endpoints.GetProductDetails;

public sealed class GetProductDetailsQueryHandler(ShoplyDbContext context)
    : IQueryHandler<GetProductDetailsQuery, GetProductDetailsResponse>
{
    public async ValueTask<GetProductDetailsResponse> Handle(
        GetProductDetailsQuery query,
        CancellationToken cancellationToken)
    {
        var product = await context.ProductsQuery
                          .AsNoTracking()
                          .FirstOrDefaultAsync(p => p.Id == query.ProductId, cancellationToken)
                      ?? throw new EntityNotFoundException<Product>(query.ProductId.Value);

        return GetProductDetailsResponse.FromProduct(product);
    }
}