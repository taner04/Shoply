using Api.Features.Baskets.Endpoints.AddBasketItem;
using Api.Features.Baskets.Endpoints.DeleteBasketItem;
using Api.Features.Baskets.Endpoints.GetBasket;
using Refit;

namespace Api.Features.Baskets;

public interface IBasketEndpoint
{
    [Get("/baskets")]
    Task<HttpResponseMessage> GetBasketAsync(GetBasketQuery query, CancellationToken cancellationToken);

    [Post("/baskets/items")]
    Task<HttpResponseMessage> AddBasketItemAsync([Body] AddBasketItemCommand command,
        CancellationToken cancellationToken);

    [Delete("/baskets/items")]
    Task<HttpResponseMessage> DeleteBasketItemAsync([Body] DeleteBasketItemCommand command,
        CancellationToken cancellationToken);
}