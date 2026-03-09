using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace IntegrationTests.Infrastructure.Mocks.Jwt;

public static class JwtBearerOptionsMock
{
    internal static IServiceCollection AddMockJwtBearerOptions(
        this IServiceCollection services)
    {
        services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            var config = new OpenIdConnectConfiguration
            {
                Issuer = JwtTokenMock.Issuer
            };

            config.SigningKeys.Add(JwtTokenMock.SecurityKey);
            options.Configuration = config;
        });

        return services;
    }
}