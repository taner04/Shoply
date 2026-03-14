using BasketId = Api.Features.Baskets.Models.BasketId;
using OrderId = Api.Features.Orders.Models.OrderId;
using ProductId = Api.Features.Products.Models.ProductId;
using UserId = Api.Features.Users.Models.UserId;

namespace Api.Common.Infrastructure.Persistence.Configurations;

[EfCoreConverter<UserId>]
[EfCoreConverter<ProductId>]
[EfCoreConverter<OrderId>]
[EfCoreConverter<BasketId>]
internal sealed partial class EfcVogenIdConverter;