using Refit;
using Shoply.WebApi.Features.Products.Endpoints.CreateProduct;
using Shoply.WebApi.Features.Products.Endpoints.UpdateProduct;

namespace Shoply.WebApi.Features.Products;

public interface IProductEndpoint
{
    [Post("/products")]
    Task<HttpResponseMessage> CreateProductAsync(
        [Body] CreateProductCommand command,
        CancellationToken cancellationToken);

    [Delete("/products/{productId}")]
    Task<HttpResponseMessage> DeleteProductAsync(
        ProductId productId,
        CancellationToken cancellationToken);

    [Get("/products")]
    Task<HttpResponseMessage> GetProductsAsync(
        [Query] int pageIndex = 1,
        [Query] int pageSize = 10,
        CancellationToken cancellationToken = default);

    [Get("/products/{productId}")]
    Task<HttpResponseMessage> GetProductDetailsAsync(
        ProductId productId,
        CancellationToken cancellationToken);

    [Put("/products/{productId}")]
    Task<HttpResponseMessage> UpdateProductAsync(
        ProductId productId,
        [Body] UpdateProductCommand command,
        CancellationToken cancellationToken);
}