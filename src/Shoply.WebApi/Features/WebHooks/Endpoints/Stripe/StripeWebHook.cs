using Shoply.WebApi.Features.WebHooks.Endpoints.Stripe.EndpointFilter;

namespace Shoply.WebApi.Features.WebHooks.Endpoints.Stripe;

public sealed class StripeWebHook : IEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapPost("webhooks/payment", () => Task.FromResult(Results.Ok()))
            .AddEndpointFilter<StripeIdempotencyEndpointFilter>()
            .WithName("PaymentNotification")
            .WithTags("WebHooks");
    }
}