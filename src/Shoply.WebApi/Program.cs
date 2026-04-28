using Shoply.ServiceDefaults;
using Shoply.WebApi.Common.Composition.Configs;
using Shoply.WebApi.Common.Composition.Configs.OpenApi;
using Shoply.WebApi.Common.Composition.Extensions.ServiceCollection;

var builder = WebApplication.CreateBuilder(args);

_ = builder.AddServiceDefaults();
_ = builder.Services.AddOpenApi(OpenApiConfig.Config);
_ = builder.Services.AddProblemDetails(ProblemDetailsConfig.Config);
_ = builder.Services.AddHttpContextAccessor();
_ = builder.Services.RegisterShoplyServices(builder);

var app = builder.Build();

_ = app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    _ = app.MapOpenApi();
    _ = app.MapScalar();
    _ = app.AddHangfireDashboard();
}

_ = app.UseAuthentication();
_ = app.UseAuthorization();
_ = app.UseHttpsRedirection();
_ = app.MapEndpoints();

_ = app.Use((context, next) =>
{
    context.Request.EnableBuffering(); // Enable buffering to allow multiple reads of the request body (StripeWebHook)
    return next();
});

app.Run();