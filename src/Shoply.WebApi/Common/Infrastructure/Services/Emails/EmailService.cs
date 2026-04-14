using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Shoply.WebApi.Common.Attributes;
using Shoply.WebApi.Common.Composition.Options;

namespace Shoply.WebApi.Common.Infrastructure.Services.Emails;

[ServiceInjection<EmailService, IEmailService>(ServiceLifetime.Scoped)]
public partial class EmailService(ILogger<EmailService> logger, IOptions<EmailConfig> options) : IEmailService
{
    private const string SenderEmail = "noreply@shoply.com";
    private readonly EmailConfig _emailConfig = options.Value;

    public virtual async Task SendEmailAsync(IEmailTemplate template, CancellationToken cancellationToken)
    {
        using var client = new SmtpClient();

        try
        {
            await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.SmtpPort, SecureSocketOptions.None,
                cancellationToken);
            await client.SendAsync(
                new MimeMessage
                {
                    From = { MailboxAddress.Parse(SenderEmail) },
                    To = { MailboxAddress.Parse(template.To) },
                    Subject = template.Subject,
                    Body = new BodyBuilder { TextBody = template.Body }.ToMessageBody()
                }, cancellationToken);

            LogEmailSentToRecipient(logger, template.To, template.Subject);
        }
        catch (Exception e)
        {
            LogErrorSendingEmail(logger);
        }
        finally
        {
            await client.DisconnectAsync(true, cancellationToken);
            LogEmailClosed(logger);
        }
    }

    [LoggerMessage(LogLevel.Information, "Email sent to {recipient} with subject {subject}")]
    static partial void LogEmailSentToRecipient(ILogger<EmailService> logger, string recipient, string subject);

    [LoggerMessage(LogLevel.Error, "Error sending email")]
    static partial void LogErrorSendingEmail(ILogger<EmailService> logger);

    [LoggerMessage(LogLevel.Information, "Email Closed")]
    static partial void LogEmailClosed(ILogger<EmailService> logger);
}