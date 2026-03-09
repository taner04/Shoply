using Api.Features.Products.Endpoints;
using Refit;

namespace Api.Features.Products;

public interface IProductEndpoint
{
    [Post("/products")]
    Task<HttpResponseMessage> CreateProductAsync([Body] CreateProductCommand command, CancellationToken cancellationToken);
    
    [Delete("/products/{productId}")]
    Task<HttpResponseMessage> DeleteProductAsync(Guid productId, CancellationToken cancellationToken);
    
    [Get("/products")]
    Task<HttpResponseMessage> GetProductsAsync([Query] int pageIndex = 1, [Query] int pageSize = 10, CancellationToken cancellationToken = default);
}