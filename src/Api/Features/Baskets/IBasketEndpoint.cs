using Api.Features.Baskets.Endpoints;
using Refit;

namespace Api.Features.Baskets;

public interface IBasketEndpoint
{
    [Get("/baskets")]
    Task<HttpResponseMessage> GetBasketAsync(GetBasketQuery query, CancellationToken cancellationToken);

    [Post("/baskets/items")]
    Task<HttpResponseMessage> AddBasketItemAsync([Body] AddBasketItemCommand command, CancellationToken cancellationToken);

    [Delete("/baskets/items")]
    Task<HttpResponseMessage> DeleteBasketItemAsync([Body] DeleteBasketItem command, CancellationToken cancellationToken);
}
