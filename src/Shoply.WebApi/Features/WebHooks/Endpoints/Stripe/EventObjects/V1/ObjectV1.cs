using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Shoply.WebApi.Features.WebHooks.Endpoints.Stripe.EventObjects.V1;

public record ObjectV1(
    [property: JsonProperty("id")]
    [property: JsonPropertyName("id")]
    string Id,
    [property: JsonProperty("object")]
    [property: JsonPropertyName("object")]
    string Obj,
    [property: JsonProperty("application")]
    [property: JsonPropertyName("application")]
    object Application,
    [property: JsonProperty("automatic_payment_methods")]
    [property: JsonPropertyName("automatic_payment_methods")]
    object AutomaticPaymentMethods,
    [property: JsonProperty("cancellation_reason")]
    [property: JsonPropertyName("cancellation_reason")]
    object CancellationReason,
    [property: JsonProperty("client_secret")]
    [property: JsonPropertyName("client_secret")]
    string ClientSecret,
    [property: JsonProperty("created")]
    [property: JsonPropertyName("created")]
    int? Created,
    [property: JsonProperty("customer")]
    [property: JsonPropertyName("customer")]
    object Customer,
    [property: JsonProperty("description")]
    [property: JsonPropertyName("description")]
    object Description,
    [property: JsonProperty("flow_directions")]
    [property: JsonPropertyName("flow_directions")]
    object FlowDirections,
    [property: JsonProperty("last_setup_error")]
    [property: JsonPropertyName("last_setup_error")]
    object LastSetupError,
    [property: JsonProperty("latest_attempt")]
    [property: JsonPropertyName("latest_attempt")]
    object LatestAttempt,
    [property: JsonProperty("livemode")]
    [property: JsonPropertyName("livemode")]
    bool? Livemode,
    [property: JsonProperty("mandate")]
    [property: JsonPropertyName("mandate")]
    object Mandate,
    [property: JsonProperty("metadata")]
    [property: JsonPropertyName("metadata")]
    Dictionary<object, object> Metadata,
    [property: JsonProperty("next_action")]
    [property: JsonPropertyName("next_action")]
    object NextAction,
    [property: JsonProperty("on_behalf_of")]
    [property: JsonPropertyName("on_behalf_of")]
    object OnBehalfOf,
    [property: JsonProperty("payment_method")]
    [property: JsonPropertyName("payment_method")]
    string PaymentMethod,
    [property: JsonProperty("payment_method_options")]
    [property: JsonPropertyName("payment_method_options")]
    PaymentMethodOptionsV1 PaymentMethodOptions,
    [property: JsonProperty("payment_method_types")]
    [property: JsonPropertyName("payment_method_types")]
    IReadOnlyList<string> PaymentMethodTypes,
    [property: JsonProperty("single_use_mandate")]
    [property: JsonPropertyName("single_use_mandate")]
    object SingleUseMandate,
    [property: JsonProperty("status")]
    [property: JsonPropertyName("status")]
    string Status,
    [property: JsonProperty("usage")]
    [property: JsonPropertyName("usage")]
    string Usage
)
{
    public bool TryGetMetadata(
        out OrderId orderId,
        out UserId userId,
        out string paymentIntentId,
        out Guid idempotencyKey)
    {
        if (Metadata.TryGetValue("orderId", out var orderIdObj) &&
            Metadata.TryGetValue("userId", out var userIdObj) &&
            Metadata.TryGetValue("paymentIntentId", out var paymentIntentIdObj) &&
            Metadata.TryGetValue("idempotencyKey", out var idempotencyKeyObj) &&
            Guid.TryParse(orderIdObj.ToString(), out var orderGuid) &&
            Guid.TryParse(userIdObj.ToString(), out var userGuid) &&
            Guid.TryParse(idempotencyKeyObj.ToString(), out var idempotencyGuid))
        {
            orderId = OrderId.From(orderGuid);
            userId = UserId.From(userGuid);
            paymentIntentId = paymentIntentIdObj.ToString()!;
            idempotencyKey = idempotencyGuid;
            return true;
        }

        orderId = OrderId.From(Guid.Empty);
        userId = UserId.From(Guid.Empty);
        paymentIntentId = string.Empty;
        idempotencyKey = Guid.Empty;
        return false;
    }
}