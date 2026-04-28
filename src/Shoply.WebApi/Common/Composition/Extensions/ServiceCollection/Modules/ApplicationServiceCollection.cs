using FluentValidation;
using Shoply.WebApi.Common.Behaviors;
using Shoply.WebApi.Common.Infrastructure.Services;
using Shoply.WebApi.Common.Infrastructure.Services.Emails;
using Shoply.WebApi.Common.Infrastructure.Services.Pagination;
using Shoply.WebApi.Features.Orders.Endpoints.GetOrders;
using Shoply.WebApi.Features.Products.Endpoints.GetProducts;

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

            services.AddScoped<EmailService>();
            services.AddSingleton<IMapper<Product, ProductsResponse>, ProductsMapper>();
            services.AddSingleton<IMapper<Order, OrdersResponse>, GetOrdersMapper>();
            services.AddScoped<PaginationService>();
            
            return services;
        }
    }
}