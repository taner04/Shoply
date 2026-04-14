using Microsoft.AspNetCore.OpenApi;

namespace Shoply.WebApi.Common.Composition.Configs.OpenApi;

public static class OpenApiConfig
{
    public static void Config(
        OpenApiOptions options)
    {
        options.AddDocumentTransformer<BearerDocumentTransformer>();
    }
}