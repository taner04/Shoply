using Refit;
using Shoply.WebApi.Features.Baskets.Endpoints.AddBasketItem;
using Shoply.WebApi.Features.Baskets.Endpoints.DeleteBasketItem;
using Shoply.WebApi.Features.Baskets.Endpoints.GetBasket;

namespace Shoply.WebApi.Features.Baskets;

public interface IBasketEndpoint
{
    [Get("/baskets")]
    Task<HttpResponseMessage> GetBasketAsync(
        GetBasketQuery query,
        CancellationToken cancellationToken);

    [Post("/baskets/items")]
    Task<HttpResponseMessage> AddBasketItemAsync(
        [Body] AddBasketItemCommand command,
        CancellationToken cancellationToken);

    [Delete("/baskets/items")]
    Task<HttpResponseMessage> DeleteBasketItemAsync(
        [Body] DeleteBasketItemCommand command,
        CancellationToken cancellationToken);
}