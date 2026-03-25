using Api.Common.Shared.Guards;
using Api.Common.Shared.Models;
using Api.Features.Baskets.Models;

namespace Api.Features.Users.Models;

[ValueObject<Guid>]
public readonly partial struct UserId;

public sealed class User : Entity<UserId>
{
    private readonly List<Order> _orders = [];

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
        email = email.Trim().ToLowerInvariant();
        Guard.Against.InvalidEmail<User>(email);
        Guard.Against.NullOrEmpty<User>(auth0Id);

        return new User(email, auth0Id);
    }

    public void AddOrder(Order order)
    {
        _orders.Add(order);
    }
}