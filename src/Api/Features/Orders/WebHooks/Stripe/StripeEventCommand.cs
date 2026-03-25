using Stripe;

namespace Api.Features.Orders.WebHooks.Stripe;

public record StripeEventCommand : ICommand;