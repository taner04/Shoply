using System.Security.Claims;
using Api.Common.Infrastructure.Persistence;
using Api.Common.Infrastructure.Services;
using Mediator;

namespace Api.Common.Behaviors;

public sealed class UserProvisioningBehavior<TMessage, TResponse>(
    ILogger<UserProvisioningBehavior<TMessage, TResponse>> logger,
    CurrentUserService currentUserService,
    ApplicationDbContext context,
    IHttpContextAccessor accessor
) : IPipelineBehavior<TMessage, TResponse>
    where TMessage : IMessage
{
    public async ValueTask<TResponse> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResponse> next,
        CancellationToken cancellationToken)
    {
        var auth0Id = currentUserService.GetAuth0Id();

        var user = await context.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Auth0Id == auth0Id, cancellationToken);

        if (user is null)
        {
            user = User.Create(
                currentUserService.GetClaimValue<string>(ClaimTypes.Email),
                auth0Id
            );

            context.Users.Add(user);
            await context.SaveChangesAsync(cancellationToken);
        }

        accessor.HttpContext!.Items["UserId"] = user.Id;

        return await next(message, cancellationToken);
    }
}