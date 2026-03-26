using Api.Features.Products.Endpoints.CreateProduct;
using Api.Features.Products.Endpoints.GetProductDetails;
using Api.Features.Products.Endpoints.UpdateProduct;
using Refit;

namespace Api.Features.Products;

public interface IProductEndpoint
{
    [Post("/products")]
    Task<HttpResponseMessage> CreateProductAsync([Body] CreateProductCommand command,
        CancellationToken cancellationToken);

    [Delete("/products/{productId}")]
    Task<HttpResponseMessage> DeleteProductAsync(Guid productId, CancellationToken cancellationToken);

    [Get("/products")]
    Task<HttpResponseMessage> GetProductsAsync([Query] int pageIndex = 1, [Query] int pageSize = 10,
        CancellationToken cancellationToken = default);

    [Get("/products/{productId}")]
    Task<HttpResponseMessage> GetProductDetailsAsync(Guid productId, CancellationToken cancellationToken);

    [Put("/products/{productId}")]
    Task<HttpResponseMessage> UpdateProductAsync(Guid productId, [Body] UpdateProductCommand command,
        CancellationToken cancellationToken);
}