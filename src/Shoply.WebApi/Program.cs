using Shoply.ServiceDefaults;
using Shoply.WebApi.Common.Composition.Configs;
using Shoply.WebApi.Common.Composition.Configs.OpenApi;
using Shoply.WebApi.Common.Composition.Extensions.ServiceCollection;
using Shoply.WebApi.Common.Composition.Options;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOptions<Auth0Config>()
    .BindConfiguration(nameof(Auth0Config))
    .ValidateDataAnnotations();

builder.Services.AddOpenApi(OpenApiConfig.Config);

builder.Services.AddProblemDetails(ProblemDetailsConfig.Config);
builder.Services.AddHttpContextAccessor();

builder.Services.RegisterServices(builder);

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalar();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.MapEndpoints();

app.Use((context, next) =>
{
    context.Request.EnableBuffering();
    return next();
});

app.Run();

namespace Shoply.WebApi
{
    public class Program; // for integration testing purposes
}