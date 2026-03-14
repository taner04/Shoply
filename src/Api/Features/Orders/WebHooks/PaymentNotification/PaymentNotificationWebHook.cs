using Api.Common.Abstractions;
using Api.Common.Composition.Extensions;
using Api.Common.Composition.Options;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;

namespace Api.Features.Orders.WebHooks.PaymentNotification;

public sealed class PaymentNotificationWebHook : IEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapPost("webhooks/payment", async ([FromServices] IMediator mediator,
                [FromServices] IHttpContextAccessor httpContextAccessor,
                [FromServices] IOptions<StripeConfig> stripeConfig) =>
            {
                var httpRequest = httpContextAccessor.HttpContext!.Request;
                var json = await new StreamReader(httpRequest.Body).ReadToEndAsync();
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    httpRequest.Headers["Stripe-Signature"],
                    stripeConfig.Value.WebhookSecret
                );

                await mediator.Send(new PaymentNotificationCommand(stripeEvent));
                return Results.Ok();
            })
            .WithName("PaymentNotification")
            .WithTags("WebHooks")
            .ProducesApiProblemDetails();
    }
}