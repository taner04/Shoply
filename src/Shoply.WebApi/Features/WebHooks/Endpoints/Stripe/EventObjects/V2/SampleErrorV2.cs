using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Shoply.WebApi.Features.WebHooks.Endpoints.Stripe.EventObjects.V2;

public record SampleErrorV2(
    [property: JsonProperty("error_message")]
    [property: JsonPropertyName("error_message")]
    string ErrorMessage,
    [property: JsonProperty("request")]
    [property: JsonPropertyName("request")]
    RequestV2 Request
);