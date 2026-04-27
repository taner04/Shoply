using Shoply.WebApi.Common.Infrastructure.Services.Emails.Templates;

namespace Shoply.WebApi.Features.WebHooks.Endpoints.Stripe.EmailTemplates.CheckoutSession;

internal sealed class CheckoutSessionFailedEmailTemplate(string userEmail) : UserEmailTemplate(userEmail)
{
    public override string Subject => "Your checkout session has failed";
    protected override string BuildBody() => throw new NotImplementedException();
}