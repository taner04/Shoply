using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Shoply.WebApi.Features.Orders.WebHooks.Stripe.EventObjects.V2;

public record ErrorTypeV2(
    [property: JsonProperty("code")]
    [property: JsonPropertyName("code")] string Code,
    [property: JsonProperty("error_count")]
    [property: JsonPropertyName("error_count")] int? ErrorCount,
    [property: JsonProperty("sample_errors")]
    [property: JsonPropertyName("sample_errors")] IReadOnlyList<SampleErrorV2> SampleErrors
);