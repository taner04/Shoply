using System.Net;
using Api.Common.Shared.Exceptions;

namespace Api.Common.Domain.Products.Exceptions;

public sealed class ProductInsufficientStockException(Guid productId, int requestedQuantity, int availableStock)
    : ApiException(
        $"Insufficient stock for product {productId}.",
        $"Cannot order {requestedQuantity} items. Only {availableStock} in stock.",
        "Order.Product.InsufficientStock",
        HttpStatusCode.BadRequest);