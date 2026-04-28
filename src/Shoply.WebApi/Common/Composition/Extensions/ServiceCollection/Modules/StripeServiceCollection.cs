using Shoply.WebApi.Common.Composition.Options;
using Shoply.WebApi.Common.Infrastructure.Services;
using Shoply.WebApi.Features.WebHooks.Endpoints.Stripe;
using Shoply.WebApi.Features.WebHooks.Endpoints.Stripe.EventStrategies;
using Shoply.WebApi.Features.WebHooks.Endpoints.Stripe.EventStrategies.CheckoutEvents;
using Shoply.WebApi.Features.WebHooks.Endpoints.Stripe.EventStrategies.RefundEvents;
using Shoply.WebApi.Features.WebHooks.Endpoints.Stripe.Services;
using Stripe;
using Stripe.Checkout;

namespace Shoply.WebApi.Common.Composition.Extensions.ServiceCollection.Modules;

internal static class StripeServiceCollection
{
    extension(IServiceCollection services)
    {
        internal IServiceCollection AddShoplyStripe(IConfiguration configuration)
        {
            services.PostConfigure<StripeConfig>(config => { StripeConfiguration.ApiKey = config.SecretKey; });
           
            services.AddScoped<SessionService>();
            services.AddScoped<RefundService>();
            
            services.AddScoped<StripeEventProcessor>();
            services.AddSingleton<StripeIdempotencyService>();
            services.AddScoped<StripePaymentProvider>();
            
            services.AddScoped<IStripeEventStrategy, CheckoutSessionAsyncPaymentFailedStrategy>();
            services.AddScoped<IStripeEventStrategy, CheckoutSessionAsyncPaymentSucceededStrategy>();
            services.AddScoped<IStripeEventStrategy, CheckoutSessionCompletedStrategy>();
            services.AddScoped<IStripeEventStrategy, CheckoutSessionExpiredStrategy>();
            services.AddScoped<IStripeEventStrategy, RefundCreatedStrategy>();
            services.AddScoped<IStripeEventStrategy, RefundFailedStrategy>();
            services.AddScoped<IStripeEventStrategy, RefundUpdatedStrategy>();


            return services;
        }
    }
}