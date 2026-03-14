using Mediator;
using Stripe;

namespace Api.Features.Orders.WebHooks.PaymentNotification;

public record PaymentNotificationCommand(Event StripeEvent) : ICommand;