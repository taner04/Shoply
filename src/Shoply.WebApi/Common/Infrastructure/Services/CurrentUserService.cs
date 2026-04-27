using System.Security.Claims;
using Models_UserId = Shoply.WebApi.Features.Users.Models.UserId;

namespace Shoply.WebApi.Common.Infrastructure.Services;

public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor)
{
    public const string SubClaim = "sub";
    public const string RoleClaim = "permissions";

    private HttpContext HttpContext => httpContextAccessor.HttpContext
                                       ?? throw new InvalidOperationException("HTTP context is not available.");

    private ClaimsPrincipal User => HttpContext.User;

    public string GetAuth0Id() => GetClaimValue<string>(ClaimTypes.NameIdentifier);

    public Models_UserId GetCurrentUserId()
    {
        if (httpContextAccessor.HttpContext!.Items.TryGetValue("UserId", out var id)
            && id is Models_UserId userId)
        {
            return userId;
        }

        throw new UnauthorizedAccessException("User is not authenticated.");
    }

    public T GetClaimValue<T>(
        string claimType)
    {
        var claimValue =
            User.FindFirst(claimType)?.Value ??
            throw new UnauthorizedAccessException($"Claim '{claimType}' is missing.");

        return (T)Convert.ChangeType(claimValue, typeof(T));
    }
}