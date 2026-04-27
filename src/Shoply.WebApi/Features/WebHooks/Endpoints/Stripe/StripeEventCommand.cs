using Shoply.WebApi.Features.WebHooks.Endpoints.Stripe.EventObjects.V1;

namespace Shoply.WebApi.Features.WebHooks.Endpoints.Stripe;

public record StripeEventCommand(string EventType, StripeEventObjectV1 EventObjectV1) : ICommand;