using Api.Common.Behaviors;
using Api.Common.Behaviors.Logger;
using Api.Common.Domain.Users;
using Api.Common.Infrastructure.Persistence;
using Api.Common.Infrastructure.Persistence.Interceptors;
using Api.Common.Infrastructure.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using ServiceDefaults;

namespace Api.Common.Composition.Extensions;

public static class ServiceCollectionExtensions
{
    extension(
        IServiceCollection services)
    {
        public IServiceCollection AddAuthenticationAndAuthorization(
            IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Authority = $"https://{configuration["Auth0:Domain"]}";
                options.Audience = configuration["Auth0:Audience"];

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidAudience = configuration["Auth0:Audience"],
                    ValidIssuer = $"https://{configuration["Auth0:Domain"]}/",
                    RoleClaimType = $"{configuration["Auth0:Audience"]}/roles"
                };
            });

            services.AddAuthorizationBuilder()
                .AddPolicy(Policies.User, policy => policy.RequireRole(Policies.User))
                .AddPolicy(Policies.Admin, policy => policy.RequireRole(Policies.Admin));

            return services;
        }
        
        public IServiceCollection AddInfrastructure(WebApplicationBuilder builder)
        {
            services.AddScoped<CurrentUserService>();
            services.AddScoped<ISaveChangesInterceptor, AuditableInterceptor>();

            services.AddDbContext<ApplicationDbContext>((
                sp,
                opt) =>
            {
                opt.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());

                if (builder.Environment.IsDevelopment())
                {
                    opt.EnableSensitiveDataLogging();
                    opt.EnableDetailedErrors();
                }

                opt.UseNpgsql(builder.Configuration.GetConnectionString(AppHostConstants.Database));
            });
            
            return services;
        }

        public IServiceCollection AddApplication()
        {
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

            return services;
        }
    }
}