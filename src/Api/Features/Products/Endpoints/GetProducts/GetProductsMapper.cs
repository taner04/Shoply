using Api.Common.Abstractions;
using Api.Common.Attributes;

namespace Api.Features.Products.Endpoints.GetProducts;

[ServiceInjection(ServiceLifetime.Singleton)]
public sealed class GetProductsMapper : IMapper<Product, GetProductsResponse>
{
    public List<GetProductsResponse> Map(List<Product> source)
    {
        return source.Select(p => new GetProductsResponse(
            p.Id.Value,
            p.Name,
            p.Price,
            p.Description,
            p.Quantity,
            p.ImageUrl)).ToList();
    }
}