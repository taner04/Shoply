using Shoply.WebApi.Features.Baskets.Models;
using Shoply.WebApi.Features.WebHooks.Models;

namespace Shoply.WebApi.Common.Infrastructure.Persistence.Configurations;

[EfCoreConverter<UserId>]
[EfCoreConverter<ProductId>]
[EfCoreConverter<OrderId>]
[EfCoreConverter<PaymentId>]
[EfCoreConverter<BasketId>]
[EfCoreConverter<WebHookEventId>]
internal sealed partial class EfcVogenIdConverter;