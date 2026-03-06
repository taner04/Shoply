using Microsoft.AspNetCore.OpenApi;

namespace Api.Common.Composition.Configs.OpenApi;

public static class OpenApiConfig
{
    public static void Config(
        OpenApiOptions options)
    {
        options.AddDocumentTransformer<BearerDocumentTransformer>();
    }
}