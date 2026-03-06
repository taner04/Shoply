using Api.Common.Domain.Users;

namespace IntegrationTests.Factories;

internal static class UserFactory
{
    public const string Email = "userfactory@mail.com";
    public const string Sub = "auth0|userfactory123";

    internal static User Create()
    {
        return User.Create(Email, Sub);
    }
}