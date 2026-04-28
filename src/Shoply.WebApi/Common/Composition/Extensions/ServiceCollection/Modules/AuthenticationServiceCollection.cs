using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Shoply.WebApi.Common.Composition.Options;
using Shoply.WebApi.Common.Infrastructure.Services;

namespace Shoply.WebApi.Common.Composition.Extensions.ServiceCollection.Modules;

internal static class AuthenticationServiceCollection
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddShoplyAuthentication(IConfiguration configuration)
        {
            var auth0Config = configuration.GetOptions<Auth0Config>();
            ArgumentNullException.ThrowIfNull(auth0Config);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Authority = $"https://{auth0Config.Domain}";
                options.Audience = auth0Config.Audience;
                options.MapInboundClaims = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidAudience = auth0Config.Audience,
                    ValidIssuer = $"https://{auth0Config.Domain}/",
                    RoleClaimType = CurrentUserService.RoleClaim,
                    NameClaimType = CurrentUserService.SubClaim
                };
            });

            services.AddAuthorizationBuilder()
                .AddPolicy(Security.Policies.Admin,
                    policy => policy.RequireClaim("permissions", "admin:create", "admin:read"))
                .AddPolicy(Security.Policies.User,
                    policy => policy.RequireClaim("permissions", "user:create", "user:read"));

            return services;
        }
    }
}