using Shoply.WebApi.Common.Shared.Guards;
using Shoply.WebApi.Common.Shared.Models;
using Shoply.WebApi.Features.Baskets.Models;

namespace Shoply.WebApi.Features.Users.Models;

[ValueObject<Guid>]
public readonly partial struct UserId
{
    private static Validation Validate(Guid value)
    {
        return value != Guid.Empty ? Validation.Ok : Validation.Invalid("ProductId must set to non-default value.");
    }
}

public sealed class User : Entity<UserId>
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private User()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    } // For EF Core

    public User(string email, string auth0Id)
    {
        Guard.Against.InvalidEmail<User>(email);
        Guard.Against.NullOrEmpty<User>(auth0Id);

        Id = UserId.From(Guid.CreateVersion7());
        Email = email;
        Auth0Id = auth0Id;
        Basket = new Basket();
    }

    public string Email { get; set; }
    public string Auth0Id { get; set; }
    public Basket Basket { get; set; }
    public ICollection<Order> Orders { get; init; } = [];
}