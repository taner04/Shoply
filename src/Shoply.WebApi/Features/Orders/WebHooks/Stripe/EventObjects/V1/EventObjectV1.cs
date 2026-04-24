using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Shoply.WebApi.Features.Orders.WebHooks.Stripe.EventObjects.V1;

public record EventObjectV1(
        [property: JsonProperty("id")]
        [property: JsonPropertyName("id")] string Id,
        [property: JsonProperty("object")]
        [property: JsonPropertyName("object")] string Object,
        [property: JsonProperty("api_version")]
        [property: JsonPropertyName("api_version")] string ApiVersion,
        [property: JsonProperty("created")]
        [property: JsonPropertyName("created")] int? Created,
        [property: JsonProperty("data")]
        [property: JsonPropertyName("data")] DataV1 Data,
        [property: JsonProperty("livemode")]
        [property: JsonPropertyName("livemode")] bool? Livemode,
        [property: JsonProperty("pending_webhooks")]
        [property: JsonPropertyName("pending_webhooks")] int? PendingWebhooks,
        [property: JsonProperty("request")]
        [property: JsonPropertyName("request")] RequestV1 Request,
        [property: JsonProperty("type")]
        [property: JsonPropertyName("type")] string Type
    );

