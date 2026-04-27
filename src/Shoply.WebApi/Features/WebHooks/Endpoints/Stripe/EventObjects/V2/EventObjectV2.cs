using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Shoply.WebApi.Features.WebHooks.Endpoints.Stripe.EventObjects.V2;

public record StripeEventObjectV2(
    [property: JsonProperty("id")]
    [property: JsonPropertyName("id")]
    string Id,
    [property: JsonProperty("object")]
    [property: JsonPropertyName("object")]
    string Object,
    [property: JsonProperty("context")]
    [property: JsonPropertyName("context")]
    object Context,
    [property: JsonProperty("created")]
    [property: JsonPropertyName("created")]
    DateTime? Created,
    [property: JsonProperty("data")]
    [property: JsonPropertyName("data")]
    DataV2 Data,
    [property: JsonProperty("livemode")]
    [property: JsonPropertyName("livemode")]
    bool? Livemode,
    [property: JsonProperty("reason")]
    [property: JsonPropertyName("reason")]
    object Reason,
    [property: JsonProperty("related_object")]
    [property: JsonPropertyName("related_object")]
    RelatedObjectV2 RelatedObject,
    [property: JsonProperty("type")]
    [property: JsonPropertyName("type")]
    string Type
);