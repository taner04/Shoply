using System.Net;

namespace Shoply.WebApi.Common.Shared.Exceptions;

public abstract class ApiException(string title, string message, string errorCode, HttpStatusCode statusCode)
    : Exception(message)
{
    public string Title { get; init; } = title;
    public string ErrorCode { get; init; } = errorCode;
    public HttpStatusCode StatusCode { get; init; } = statusCode;
}