using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Shoply.WebApi.Features.WebHooks.Endpoints.Stripe.EventObjects.V2;

public record RequestV2(
    [property: JsonProperty("identifier")]
    [property: JsonPropertyName("identifier")]
    string Identifier
);