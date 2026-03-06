using Api.Features.Products.Endpoints;
using Refit;

namespace Api.Features.Products;

public interface IProductEndpoint
{
    [Post("/products")]
    Task<HttpResponseMessage> CreateProductAsync(CreateProductCommand command, CancellationToken cancellationToken);
}