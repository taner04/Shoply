using Shoply.WebApi.Common.Shared.Guards;
using Shoply.WebApi.Common.Shared.Models;
using Shoply.WebApi.Features.Baskets.Models;

namespace Shoply.WebApi.Features.Users.Models;

[ValueObject<Guid>]
public readonly partial struct UserId
{
    private static Validation Validate(Guid value)
        => value != Guid.Empty ? Validation.Ok : Validation.Invalid("ProductId must set to non-default value.");
}

public sealed class User : Entity<UserId>
{
    private readonly List<Order> _orders = [];

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private User()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
        
    } // For EF Core
    
    private User(string email, string auth0Id)
    {
        Id = UserId.From(Guid.CreateVersion7());
        Email = email;
        Auth0Id = auth0Id;
        Basket = Basket.CreateEmpty();
    }

    public string Email { get; private set; }
    public string Auth0Id { get; private set; }
    public Basket Basket { get; private set; }
    public IReadOnlyCollection<Order> Orders => _orders.AsReadOnly();

    public static User Create(string email, string auth0Id)
    {
        email = email.Trim();
        Guard.Against.InvalidEmail<User>(email);
        Guard.Against.NullOrEmpty<User>(auth0Id);

        return new User(email, auth0Id);
    }

    public void AddOrder(Order order)
    {
        _orders.Add(order);
    }
}