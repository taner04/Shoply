using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Shoply.WebApi.Features.Orders.WebHooks.Stripe.EventObjects.V1;

public record DataV1(
    [property: JsonProperty("object")]
    [property: JsonPropertyName("object")] ObjectV1 Object
);