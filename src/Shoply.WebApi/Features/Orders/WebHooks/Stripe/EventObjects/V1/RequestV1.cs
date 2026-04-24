using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Shoply.WebApi.Features.Orders.WebHooks.Stripe.EventObjects.V1;

public record RequestV1(
    [property: JsonProperty("id")]
    [property: JsonPropertyName("id")] object Id,
    [property: JsonProperty("idempotency_key")]
    [property: JsonPropertyName("idempotency_key")] object IdempotencyKey
);