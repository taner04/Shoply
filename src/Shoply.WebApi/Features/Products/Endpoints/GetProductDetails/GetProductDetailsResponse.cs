namespace Shoply.WebApi.Features.Products.Endpoints.GetProductDetails;

public sealed record GetProductDetailsResponse(
    Guid Id,
    string Name,
    decimal Price,
    string Description,
    int Quantity,
    string ImageUrl)
{
    public static GetProductDetailsResponse FromProduct(Product product) =>
        new(
            product.Id.Value,
            product.Name,
            product.Price,
            product.Description,
            product.Quantity,
            product.ImageUrl);
}