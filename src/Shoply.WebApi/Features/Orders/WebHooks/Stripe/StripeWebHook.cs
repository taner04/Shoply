namespace Shoply.WebApi.Features.Orders.WebHooks.Stripe;

public sealed class StripeWebHook : IEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapPost("webhooks/payment", async ([FromServices] IMediator mediator) =>
            {
                await mediator.Send(new StripeEventCommand());
                return Results.Ok();
            })
            .WithName("PaymentNotification")
            .WithTags("WebHooks")
            .ProducesApiProblemDetails();
    }
}