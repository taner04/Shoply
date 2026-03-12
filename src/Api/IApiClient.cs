using Api.Features.Baskets;
using Api.Features.Orders;
using Api.Features.Products;

namespace Api;

public interface IApiClient : IProductEndpoint, IBasketEndpoint, IOrderEndpoint;