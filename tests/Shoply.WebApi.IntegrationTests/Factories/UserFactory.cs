using Shoply.WebApi.Features.Users.Models;

namespace Shoply.WebApi.IntegrationTests.Factories;

internal static class UserFactory
{
    public const string Email = "userfactory@mail.com";
    public const string Sub = "auth0|userfactory123";

    internal static User Create() => User.Create(Email, Sub);
}