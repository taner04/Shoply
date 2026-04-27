using Shoply.WebApi.Common.Composition.Options;
using Stripe;
using Stripe.Checkout;

namespace Shoply.WebApi.Common.Composition.Extensions.ServiceCollection.Modules;

internal static class StripeServiceCollection
{
    extension(IServiceCollection services)
    {
        internal IServiceCollection AddShoplyStripe(IConfiguration configuration)
        {
            services.AddOptions<StripeConfig>()
                .BindConfiguration(nameof(StripeConfig))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.PostConfigure<StripeConfig>(config => { StripeConfiguration.ApiKey = config.SecretKey; });

            services.AddScoped<SessionService>();
            services.AddScoped<RefundService>();

            return services;
        }
    }
}