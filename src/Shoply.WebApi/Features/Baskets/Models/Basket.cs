using Shoply.WebApi.Common.Shared.Models;

namespace Shoply.WebApi.Features.Baskets.Models;

[ValueObject<Guid>]
public readonly partial struct BasketId
{
    private static Validation Validate(Guid value)
    {
        return value != Guid.Empty ? Validation.Ok : Validation.Invalid("BasketId must set to non-default value.");
    }
}

public sealed class Basket : Entity<BasketId>
{
    public Basket()
    {
        Id = BasketId.From(Guid.CreateVersion7());
    } // For EF Core

    public ICollection<BasketItem> BasketItems { get; init; } = [];
}