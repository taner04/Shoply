using Api.Common.Abstractions;
using Mediator;

namespace Api.Features.Baskets.Endpoints.GetBasket;

public sealed record GetBasketQuery : IQuery<BasketResponse>, IUserRequest;