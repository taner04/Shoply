using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Shoply.WebApi.Features.Orders.WebHooks.Stripe.EventObjects.V2;

public record RequestV2(
    [property: JsonProperty("identifier")]
    [property: JsonPropertyName("identifier")] string Identifier
);