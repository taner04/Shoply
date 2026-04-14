using Microsoft.Extensions.DependencyInjection.Extensions;
using Shoply.WebApi.Common.Infrastructure.Persistence.Interceptors;

namespace Shoply.WebApi.IntegrationTests.Infrastructure.Mocks.Database;

public static class ApplicationDbContextMock
{
    internal static IServiceCollection AddMockDbContext(
        this IServiceCollection services,
        DbConnection connection)
    {
        services.RemoveAll<ShoplyDbContext>();
        services.RemoveAll<DbContextOptions<ShoplyDbContext>>();

        var interceptorDescriptors = services
            .Where(d => d.ServiceType == typeof(AuditableInterceptor)
                        || d.ImplementationType == typeof(AuditableInterceptor)
                        || d.ImplementationInstance is AuditableInterceptor)
            .ToList();

        foreach (var d in interceptorDescriptors)
        {
            services.Remove(d);
        }

        services.AddDbContext<ShoplyDbContext>(opt =>
        {
            opt.AddInterceptors(new ApplicationDbContextAuditableInterceptorMock());
            opt.EnableSensitiveDataLogging();
            opt.EnableDetailedErrors();
            opt.UseNpgsql(connection.ConnectionString);
        });

        return services;
    }
}