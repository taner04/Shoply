using Shoply.WebApi.Common.Attributes;

namespace Shoply.WebApi.Features.Products.Endpoints.GetProducts;

[ServiceInjection(ServiceLifetime.Singleton)]
public sealed class GetProductsMapper : IMapper<Product, GetProductsResponse>
{
    public List<GetProductsResponse> Map(List<Product> source)
    {
        return source.Select(p => new GetProductsResponse(
            p.Id.Value,
            p.Name,
            p.Price,
            p.ImageUrl)).ToList();
    }
}