using Api.Common.Composition.Options;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;

namespace Api.Common.Composition.Configs.OpenApi;

internal sealed class BearerDocumentTransformer(IOptions<Auth0Config> auth0Options) : IOpenApiDocumentTransformer
{
    private readonly Auth0Config _auth0Config = auth0Options.Value;

    public Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();

        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

        document.Components.SecuritySchemes["JWT"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "JWT Bearer Token"
        };

        document.Components.SecuritySchemes["OAuth2"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Description = "Auth0 OAuth2 Login",
            Flows = new OpenApiOAuthFlows
            {
                AuthorizationCode = new OpenApiOAuthFlow
                {
                    AuthorizationUrl = new Uri($"https://{_auth0Config.Domain}/authorize"),
                    TokenUrl = new Uri($"https://{_auth0Config.Domain}/oauth/token"),
                    Scopes = new Dictionary<string, string>()
                }
            }
        };

        return Task.CompletedTask;
    }
}