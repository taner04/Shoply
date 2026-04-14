using Shoply.WebApi.Features.Baskets;
using Shoply.WebApi.Features.Orders;
using Shoply.WebApi.Features.Products;

namespace Shoply.WebApi;

public interface IApiClient : IProductEndpoint, IBasketEndpoint, IOrderEndpoint;