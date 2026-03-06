using Api.Common.Domain.Baskets;
using Api.Common.Domain.Orders;
using Api.Common.Domain.Products;
using Api.Common.Domain.Users;
using Vogen;

namespace Api.Common.Infrastructure.Persistence.Configurations;

[EfCoreConverter<UserId>]
[EfCoreConverter<ProductId>]
[EfCoreConverter<OrderId>]
[EfCoreConverter<OrderItemId>]
[EfCoreConverter<BasketId>]
[EfCoreConverter<BasketItemId>]
internal sealed partial class EfcVogenIdConverter;