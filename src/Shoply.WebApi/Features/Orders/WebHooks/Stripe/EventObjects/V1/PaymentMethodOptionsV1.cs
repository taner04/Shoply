using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Shoply.WebApi.Features.Orders.WebHooks.Stripe.EventObjects.V1;

public record PaymentMethodOptionsV1(
    [property: JsonProperty("acss_debit")]
    [property: JsonPropertyName("acss_debit")] AcssDebitV1 AcssDebit
);