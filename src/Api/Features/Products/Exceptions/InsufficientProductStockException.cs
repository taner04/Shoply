using System.Net;

namespace Api.Features.Products.Exceptions;

public sealed class InsufficientProductStockException(Guid productId, int requestedQuantity, int availableStock)
    : ApiException(
        $"Insufficient stock for product {productId}.",
        $"Cannot order {requestedQuantity} items. Only {availableStock} in stock.",
        "Product.Stock.Insufficient",
        HttpStatusCode.BadRequest);