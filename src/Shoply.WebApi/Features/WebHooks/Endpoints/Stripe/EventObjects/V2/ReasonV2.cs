using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Shoply.WebApi.Features.WebHooks.Endpoints.Stripe.EventObjects.V2;

public record ReasonV2(
    [property: JsonProperty("error_count")]
    [property: JsonPropertyName("error_count")]
    int? ErrorCount,
    [property: JsonProperty("error_types")]
    [property: JsonPropertyName("error_types")]
    IReadOnlyList<ErrorTypeV2> ErrorTypes
);