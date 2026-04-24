using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Shoply.WebApi.Features.Orders.WebHooks.Stripe.EventObjects.V2;

public record RelatedObjectV2(
    [property: JsonProperty("id")]
    [property: JsonPropertyName("id")] string Id,
    [property: JsonProperty("type")]
    [property: JsonPropertyName("type")] string Type,
    [property: JsonProperty("url")]
    [property: JsonPropertyName("url")] string Url
);