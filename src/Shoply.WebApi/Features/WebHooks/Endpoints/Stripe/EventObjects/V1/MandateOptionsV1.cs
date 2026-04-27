using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Shoply.WebApi.Features.WebHooks.Endpoints.Stripe.EventObjects.V1;

public record MandateOptionsV1(
    [property: JsonProperty("interval_description")]
    [property: JsonPropertyName("interval_description")]
    string IntervalDescription,
    [property: JsonProperty("payment_schedule")]
    [property: JsonPropertyName("payment_schedule")]
    string PaymentSchedule,
    [property: JsonProperty("transaction_type")]
    [property: JsonPropertyName("transaction_type")]
    string TransactionType
);