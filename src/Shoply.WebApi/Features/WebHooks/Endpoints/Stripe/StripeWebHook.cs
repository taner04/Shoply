using Shoply.WebApi.Features.WebHooks.Endpoints.Stripe.EndpointFilter.Idempotency;

namespace Shoply.WebApi.Features.WebHooks.Endpoints.Stripe;

public sealed class StripeWebHook : IEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapPost("webhooks/payment",
                async ([FromBody] StripeEventCommand command, [FromServices] IMediator mediator) =>
                {
                    await mediator.Send(command);
                    return Results.Ok();
                })
            .AddEndpointFilter<IdempotencyEndpointFilter>()
            .WithName("PaymentNotification")
            .WithTags("WebHooks")
            .ProducesApiProblemDetails();
    }
}