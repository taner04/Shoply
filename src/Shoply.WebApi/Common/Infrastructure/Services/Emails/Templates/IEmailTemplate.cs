namespace Shoply.WebApi.Common.Infrastructure.Services.Emails.Templates;

public interface IEmailTemplate
{
    string To { get; }
    string Subject { get; }
    string Body { get; }
}