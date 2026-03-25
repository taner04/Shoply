namespace Api.Common.Infrastructure.Services.Emails;

public interface IEmailTemplate
{
    string To { get; }
    string Subject { get; }
    string Body { get; }
}