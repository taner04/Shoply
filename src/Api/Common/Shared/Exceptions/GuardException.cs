using System.Net;

namespace Api.Common.Shared.Exceptions;

public sealed class GuardException : ApiException
{
    private GuardException(string title, string message, string errorCode)
        : base(title, message, errorCode, HttpStatusCode.BadRequest)
    {
    }

    public static void Throw(string ownerName, string propertyName, string rule, string message)
    {
        throw new GuardException(
            "Invalid argument",
            message,
            $"{ownerName}.{propertyName}.{rule}");
    }
}