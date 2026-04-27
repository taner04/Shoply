using Shoply.WebApi.Common.Composition.Extensions.ServiceCollection.Modules;

namespace Shoply.WebApi.Common.Composition.Extensions.ServiceCollection;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection RegisterServices(WebApplicationBuilder builder)
        {
            services.AddShoplyDbContext(builder);
            services.AddShoplyApplicationServices();
            services.AddShoplyStripe(builder.Configuration);
            services.AddShoplyAuthentication(builder.Configuration);
            services.AddShoplyRateLimiting();

            return services;
        }
    }
}