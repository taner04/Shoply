using Microsoft.Extensions.Options;
using Shoply.WebApi.Common.Composition.Options;
using Stripe;

namespace Shoply.WebApi.Features.WebHooks.Endpoints.Stripe.EndpointFilter.Idempotency;

internal sealed class IdempotencyEndpointFilter(
    IdempotencyService idempotencyService,
    IOptions<StripeConfig> options) : IEndpointFilter
{
    private readonly StripeConfig _stripeConfig = options.Value;

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        try
        {
            var body = context.HttpContext.Request.Body;
            body.Seek(0, SeekOrigin.Begin);

            using var bodyReader = new StreamReader(body);
            var rawBody = await bodyReader.ReadToEndAsync();

            var stripeEvent = EventUtility.ConstructEvent(
                rawBody,
                context.HttpContext.Request.Headers["Stripe-Signature"],
                _stripeConfig.WebhookSecret
            );

            await idempotencyService.ProcessEventIdempotentlyAsync(
                stripeEvent,
                async () => await next(context)
            );

            return Results.Ok();
        }
        catch (StripeException e)
        {
            return Results.BadRequest();
        }
        catch (Exception e)
        {
            return Results.StatusCode(500);
        }
    }
}