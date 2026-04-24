using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Shoply.WebApi.Common.Composition.Options;
using Stripe;

namespace Shoply.WebApi.Features.Orders.WebHooks.Stripe.EndpointFilter;

internal sealed class IdempotencyEndpointFilter(IDistributedCache cache, IOptions<StripeConfig> options) : IEndpointFilter
{
    private const int CasheAbsoluteExpirationInMinutes = 60;
    private const int CasheSlidingExpirationInMinutes = 60;
    
    private readonly StripeConfig _stripeConfig = options.Value;
    private readonly DistributedCacheEntryOptions _cacheEntryOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CasheAbsoluteExpirationInMinutes),
        SlidingExpiration = TimeSpan.FromMinutes(CasheSlidingExpirationInMinutes)
    };
    
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var httpRequest = context.HttpContext.Request;
        
        using var reader = new StreamReader(httpRequest.Body);

        var stripeEvent = EventUtility.ConstructEvent(
            await reader.ReadToEndAsync(),
            httpRequest.Headers["Stripe-Signature"],
            _stripeConfig.WebhookSecret
        );
        
        var cashKey = $"Idempotency-{stripeEvent.Request.IdempotencyKey}";
        
        if (!string.IsNullOrEmpty(await cache.GetStringAsync(stripeEvent.Request.IdempotencyKey)))
        {
            return Results.Ok();
        }
        
        await cache.SetStringAsync(cashKey, JsonSerializer.Serialize(stripeEvent), _cacheEntryOptions);
        
        return await next(context);
    }
}