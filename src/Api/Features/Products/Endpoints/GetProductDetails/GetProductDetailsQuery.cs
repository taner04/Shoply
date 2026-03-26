namespace Api.Features.Products.Endpoints.GetProductDetails;

public sealed record GetProductDetailsQuery(Guid ProductId) : IQuery<GetProductDetailsResponse>;
