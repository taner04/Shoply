using Stripe;

namespace Shoply.WebApi.Features.Orders.WebHooks.Stripe;

public record StripeEventCommand(string EventType, IHasMetadata Metadata) : ICommand;