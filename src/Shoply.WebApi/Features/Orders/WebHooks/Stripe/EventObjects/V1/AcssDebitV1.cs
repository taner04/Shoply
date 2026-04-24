using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Shoply.WebApi.Features.Orders.WebHooks.Stripe.EventObjects.V1;

public record AcssDebitV1(
    [property: JsonProperty("currency")]
    [property: JsonPropertyName("currency")] string Currency,
    [property: JsonProperty("mandate_options")]
    [property: JsonPropertyName("mandate_options")] MandateOptionsV1 MandateOptions,
    [property: JsonProperty("verification_method")]
    [property: JsonPropertyName("verification_method")] string VerificationMethod
);