using Api.Common.Domain.Baskets;
using Api.Common.Domain.Orders;

namespace Api.Common.Infrastructure.Persistence.Configurations;

[EfCoreConverter<UserId>]
[EfCoreConverter<ProductId>]
[EfCoreConverter<OrderId>]
[EfCoreConverter<BasketId>]
internal sealed partial class EfcVogenIdConverter;