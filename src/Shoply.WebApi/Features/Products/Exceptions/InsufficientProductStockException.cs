using System.Net;

namespace Shoply.WebApi.Features.Products.Exceptions;

public sealed class InsufficientProductStockException(ProductId productId, int requestedQuantity, int availableStock)
    : ApiException(
        $"Insufficient stock for product {productId.Value}.",
        $"Cannot order {requestedQuantity} items. Only {availableStock} in stock.",
        "Product.Stock.Insufficient",
        HttpStatusCode.BadRequest);