using Microsoft.Extensions.Options;
using Shoply.WebApi.Common.Composition.Options;
using Shoply.WebApi.Features.WebHooks.Endpoints.Stripe.Services;
using Stripe;

namespace Shoply.WebApi.Features.WebHooks.Endpoints.Stripe.EndpointFilter;

internal sealed class StripeIdempotencyEndpointFilter(
    StripeIdempotencyService stripeIdempotencyService,
    IOptions<StripeConfig> options) : IEndpointFilter
{
    private readonly StripeConfig _stripeConfig = options.Value;

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
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

        await stripeIdempotencyService.AddEventIdempotentlyAsync(stripeEvent);

        return await next(context);
    }
}