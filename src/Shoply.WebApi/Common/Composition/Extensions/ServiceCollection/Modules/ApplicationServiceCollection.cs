using FluentValidation;
using Shoply.WebApi.Common.Attributes;
using Shoply.WebApi.Common.Behaviors;
using Shoply.WebApi.Common.Behaviors.Logger;
using Shoply.WebApi.Common.Infrastructure.Services;
using Shoply.WebApi.Common.Infrastructure.Services.Emails;

namespace Shoply.WebApi.Common.Composition.Extensions.ServiceCollection.Modules;

internal static class ApplicationServiceCollection
{
    extension(IServiceCollection services)
    {
        internal IServiceCollection AddShoplyApplicationServices()
        {
            services.AddScoped<CurrentUserService>();

            services.AddMediator(options =>
            {
                options.ServiceLifetime = ServiceLifetime.Scoped;
                options.GenerateTypesAsInternal = true;
                options.PipelineBehaviors =
                [
                    typeof(LoggingBehavior<,>),
                    typeof(UserProvisioningBehavior<,>),
                    typeof(FluentValidationBehaviour<,>)
                ];
            });

            services.AddValidatorsFromAssembly(typeof(Program).Assembly);
            services.AddScoped<IEmailService, EmailService>();

            services.AddMemoryCache();
            services.AddDistributedMemoryCache();

            _ = services.RegisterAutoServices();

            return services;
        }

        private IServiceCollection RegisterAutoServices()
        {
            var assembly = typeof(Program).Assembly;

            foreach (var type in assembly.GetTypes())
            {
                var attr = type.GetCustomAttributes(false)
                    .OfType<ServiceInjectionAttribute>()
                    .FirstOrDefault();

                if (attr is null)
                {
                    continue;
                }

                var lifetime = (ServiceLifetime)(int)attr.ServiceLifetime;
                var serviceType = attr.GetType() == typeof(ServiceInjectionAttribute)
                    ? type
                    : attr.GetType().GetGenericArguments()[1];

                services.Add(new ServiceDescriptor(serviceType, type, lifetime));
            }

            return services;
        }
    }
}