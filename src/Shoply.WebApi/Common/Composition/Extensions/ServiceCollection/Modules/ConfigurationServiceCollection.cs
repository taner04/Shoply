using Shoply.WebApi.Common.Composition.Options;

namespace Shoply.WebApi.Common.Composition.Extensions.ServiceCollection.Modules;

internal static class ConfigurationServiceCollection
{
    extension(IServiceCollection services)
    {
        internal IServiceCollection AddShoplyConfiguration(WebApplicationBuilder builder)
        {
            var isTestEnv = builder.Environment.IsEnvironment("Testing");
            var configuration = builder.Configuration;

            services.AddConfig<StripeConfig>(configuration, isTestEnv);
            services.AddConfig<Auth0Config>(configuration, isTestEnv);
            services.AddConfig<EmailConfig>(configuration, isTestEnv);

            return services;
        }
        
        private IServiceCollection AddConfig<TOptions>(IConfiguration configuration, bool isTestingEnv)
            where TOptions : class
        {
            var sectionName = typeof(TOptions).Name;

            var options = services.AddOptions<TOptions>()
                .Bind(configuration.GetSection(sectionName));

            if (!isTestingEnv)
            {
                options.ValidateDataAnnotations()
                    .ValidateOnStart();
            }

            return services;
        }
    }
}