using System.ComponentModel.DataAnnotations;

namespace Api.Common.Composition.Options;

public sealed class StripeConfig
{
    [Required(ErrorMessage = "Public key is required.")]
    [MinLength(10, ErrorMessage = "Stripe public key appears to be invalid.")]
    public string PublishableKey { get; init; } = null!;

    [Required(ErrorMessage = "Private key is required.")]
    [MinLength(10, ErrorMessage = "Stripe private key appears to be invalid.")]
    public string SecretKey { get; init; } = null!;

    [Required(ErrorMessage = "Success URL is required.")]
    [Url(ErrorMessage = "Success URL must be a valid absolute URL.")]
    public string SuccessUrl { get; init; } = null!;

    [Required(ErrorMessage = "Cancel URL is required.")]
    [Url(ErrorMessage = "Cancel URL must be a valid absolute URL.")]
    public string CancelUrl { get; init; } = null!;

    [MinLength(1, ErrorMessage = "Webhook secret is required for validating Stripe webhook signatures.")]
    public string WebhookSecret { get; init; } = null!;
}