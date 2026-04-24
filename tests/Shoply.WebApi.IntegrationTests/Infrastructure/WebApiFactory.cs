using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Moq;
using Refit;
using Shoply.ServiceDefaults;
using Shoply.WebApi.Common.Infrastructure.Services.Emails;
using Shoply.WebApi.Common.Infrastructure.Services.Emails.Templates;
using Shoply.WebApi.IntegrationTests.Infrastructure.Mocks.Database;
using Shoply.WebApi.IntegrationTests.Infrastructure.Mocks.Jwt;

namespace Shoply.WebApi.IntegrationTests.Infrastructure;

public class WebApiFactory(DbConnection dbConnection)
    : WebApplicationFactory<Program>
{
    private Mock<IEmailService>? _emailServiceMock;

    public Mock<IEmailService> EmailServiceMock =>
        _emailServiceMock ?? throw new InvalidOperationException("Email mock not initialized");

    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.ConfigureLogging(opt =>
        {
            opt.ClearProviders();
            opt.AddConsole();
        });

        builder.ConfigureServices(services =>
        {
            services.AddMockDbContext(dbConnection);
            services.AddMockJwtBearerOptions();

            _emailServiceMock = new Mock<IEmailService>(MockBehavior.Loose);
            _emailServiceMock
                .Setup(x => x.SendEmailAsync(It.IsAny<IEmailTemplate>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            services.RemoveAll<IEmailService>();
            services.AddSingleton(_emailServiceMock.Object);

            services.AddRefitClient<IApiClient>();
        });

        builder.UseSetting($"ConnectionStrings:{AppHostConstants.Database}", dbConnection.ConnectionString);
    }
}