namespace Shoply.WebApi.Common.Infrastructure.Services.Emails;

public interface IEmailService
{
    Task SendEmailAsync(IEmailTemplate template, CancellationToken cancellationToken);
}