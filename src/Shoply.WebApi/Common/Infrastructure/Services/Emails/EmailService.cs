using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Shoply.WebApi.Common.Composition.Options;
using Shoply.WebApi.Common.Infrastructure.Services.Emails.Templates;

namespace Shoply.WebApi.Common.Infrastructure.Services.Emails;

public partial class EmailService(ILogger<EmailService> logger, IOptions<EmailConfig> options)
{
    private const string SenderEmail = "noreply@shoply.com";
    private readonly EmailConfig _emailConfig = options.Value;

    public async Task SendEmailAsync(IEmailTemplate template, CancellationToken cancellationToken)
    {
        using var client = new SmtpClient();

        try
        {
            await client.ConnectAsync(_emailConfig.Host, _emailConfig.Port, SecureSocketOptions.None,
                cancellationToken);
            await client.SendAsync(
                new MimeMessage
                {
                    From = { MailboxAddress.Parse(SenderEmail) },
                    To = { MailboxAddress.Parse(template.To) },
                    Subject = template.Subject,
                    Body = new BodyBuilder { HtmlBody = template.Body }.ToMessageBody()
                }, cancellationToken);

            LogEmailSentToRecipient(logger, template.To, template.Subject);
        }
        catch
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
    private static partial void LogEmailSentToRecipient(ILogger<EmailService> logger, string recipient, string subject);

    [LoggerMessage(LogLevel.Error, "Error sending email")]
    private static partial void LogErrorSendingEmail(ILogger<EmailService> logger);

    [LoggerMessage(LogLevel.Information, "Email Closed")]
    private static partial void LogEmailClosed(ILogger<EmailService> logger);
}