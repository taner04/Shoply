using Hangfire;
using Hangfire.PostgreSql;
using Shoply.ServiceDefaults;

namespace Shoply.WebApi.Common.Composition.Extensions.ServiceCollection.Modules;

internal static class HangfireServiceCollection
{
    extension(IServiceCollection services)
    {
        internal IServiceCollection AddShoplyHangfire(WebApplicationBuilder builder)
        {
            services.AddHangfire((provider, configuration) =>
            {
                configuration.UseSimpleAssemblyNameTypeSerializer();
                configuration.UseRecommendedSerializerSettings();
                configuration.UsePostgreSqlStorage(c =>
                {
                    c.UseNpgsqlConnection(builder.Configuration.GetConnectionString(AppHostConstants.Database));
                });
            });

            services.AddHangfireServer(options => { options.SchedulePollingInterval = TimeSpan.FromSeconds(1); });

            return services;
        }
    }
}