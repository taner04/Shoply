using Shoply.WebApi.Features.Baskets.Models;

namespace Shoply.WebApi.Common.Infrastructure.Persistence.Configurations;

[EfCoreConverter<UserId>]
[EfCoreConverter<ProductId>]
[EfCoreConverter<OrderId>]
[EfCoreConverter<PaymentId>]
[EfCoreConverter<BasketId>]
internal sealed partial class EfcVogenIdConverter;