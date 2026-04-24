using Shoply.WebApi.Common.Infrastructure.Services.Emails.Templates;

namespace Shoply.WebApi.Common.Infrastructure.Services.Emails;

public interface IEmailService
{
    Task SendEmailAsync(IEmailTemplate template, CancellationToken cancellationToken);
}