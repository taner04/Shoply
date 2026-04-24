using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Shoply.WebApi.Features.Orders.WebHooks.Stripe.EventObjects.V2;

public record DataV2(
    [property: JsonProperty("developer_message_summary")]
    [property: JsonPropertyName("developer_message_summary")] string DeveloperMessageSummary,
    [property: JsonProperty("reason")]
    [property: JsonPropertyName("reason")] ReasonV2 Reason,
    [property: JsonProperty("validation_end")]
    [property: JsonPropertyName("validation_end")] DateTime? ValidationEnd,
    [property: JsonProperty("validation_start")]
    [property: JsonPropertyName("validation_start")] DateTime? ValidationStart
);