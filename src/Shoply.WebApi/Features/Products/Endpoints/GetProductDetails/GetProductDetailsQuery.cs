namespace Shoply.WebApi.Features.Products.Endpoints.GetProductDetails;

public sealed record GetProductDetailsQuery(ProductId ProductId) : IQuery<GetProductDetailsResponse>;