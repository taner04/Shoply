using Microsoft.EntityFrameworkCore.Diagnostics;
using Shoply.ServiceDefaults;
using Shoply.WebApi.Common.Infrastructure.Persistence.Interceptors;

namespace Shoply.WebApi.Common.Composition.Extensions.ServiceCollection.Modules;

internal static class DbContextServiceCollection
{
    extension(IServiceCollection services)
    {
        internal IServiceCollection AddShoplyDbContext(WebApplicationBuilder builder)
        {
            var connectionString = builder.Configuration.GetConnectionString(AppHostConstants.Database);
            ArgumentNullException.ThrowIfNull(connectionString);


            services.AddScoped<ISaveChangesInterceptor, AuditableInterceptor>();
            services.AddDbContext<ShoplyDbContext>((
                sp,
                opt) =>
            {
                opt.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());

                if (builder.Environment.IsDevelopment())
                {
                    opt.EnableSensitiveDataLogging();
                    opt.EnableDetailedErrors();
                }

                opt.UseNpgsql(connectionString);
            });

            return services;
        }
    }
}