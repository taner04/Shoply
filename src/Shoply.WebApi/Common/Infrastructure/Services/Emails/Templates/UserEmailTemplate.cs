namespace Shoply.WebApi.Common.Infrastructure.Services.Emails.Templates;

public abstract class UserEmailTemplate(string userEmail) : IEmailTemplate
{
    public string To => userEmail;
    public abstract string Subject { get; }
    public string Body => BuildBody();
    
    protected abstract string BuildBody();
}