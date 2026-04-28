namespace Shoply.WebApi.Features.Products.Endpoints.GetProducts;

public sealed class ProductsMapper : IMapper<Product, ProductsResponse>
{
    public List<ProductsResponse> Map(List<Product> source)
    {
        return source.Select(p => new ProductsResponse(
            p.Id.Value,
            p.Name,
            p.Price,
            p.ImageUrl)).ToList();
    }
}