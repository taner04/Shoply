using Shoply.WebApi.Common.Composition.Extensions.ServiceCollection.Modules;

namespace Shoply.WebApi.Common.Composition.Extensions.ServiceCollection;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection RegisterShoplyServices(WebApplicationBuilder builder)
        {
            services
                .AddShoplyStripe(builder.Configuration)
                .AddShoplyAuthentication(builder.Configuration)
                .AddShoplyConfiguration(builder)
                .AddShoplyDbContext(builder)
                .AddShoplyHangfire(builder)
                .AddShoplyApplicationServices()
                .AddShoplyRateLimiting();

            return services;
        }
    }
}