using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Text.Json;

namespace Shoply.WebApi.Common.Behaviors.Logger;

public sealed class LoggerRequestContext
{
    private const string Unknown = "Unknown";

    private LoggerRequestContext(
        string path,
        string method,
        string userId,
        string userName,
        string traceId,
        string ipAddress,
        string userAgent,
        string queryString)
    {
        Path = path;
        Method = method;
        UserId = userId;
        UserName = userName;
        TraceId = traceId;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        QueryString = queryString;
    }

    public string Path { get; }
    public string Method { get; }
    public string UserId { get; }
    public string UserName { get; }
    public string TraceId { get; }
    public string IpAddress { get; }
    public string UserAgent { get; }
    public string QueryString { get; }

    public static LoggerRequestContext FromHttpContext(
        HttpContext? httpContext)
    {
        if (httpContext is null)
        {
            return CreateUnknown();
        }

        var request = httpContext.Request;
        var user = httpContext.User;

        return new LoggerRequestContext(
            request.Path.HasValue ? request.Path.Value! : Unknown,
            request.Method ?? Unknown,
            user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Unknown,
            user.Identity?.Name ?? Unknown,
            httpContext.TraceIdentifier ?? Unknown,
            httpContext.Connection?.RemoteIpAddress?.ToString() ?? Unknown,
            request.Headers.UserAgent.ToString().NullIfEmpty() ?? Unknown,
            request.QueryString.HasValue ? request.QueryString.Value! : Unknown
        );
    }

    private static LoggerRequestContext CreateUnknown() =>
        new(Unknown, Unknown, Unknown, Unknown, Unknown, Unknown, Unknown, Unknown);


    [SuppressMessage("Performance", "CA1869:Cache and reuse JsonSerializerOptions instances")]
    public override string ToString() =>
        JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
}

internal static class StringExtensions
{
    public static string? NullIfEmpty(
        this string? s) =>
        string.IsNullOrWhiteSpace(s) ? null : s;
}