using System.Net;

namespace Shoply.WebApi.Features.Baskets.Exceptions;

public class ProductOutOfStockException : ApiException
{
    private ProductOutOfStockException(string title, string message, string errorCode, HttpStatusCode statusCode) :
        base(title, message, errorCode, statusCode)
    {
    }

    public static void ThrowIfOutOfStock(Product product)
    {
        if (product.Quantity > 0)
        {
            return;
        }

        throw new ProductOutOfStockException(
            "Product Out Of Stock",
            $"The product with id {product.Id} is out of stock.",
            "Product.Quantity.OutOfStock",
            HttpStatusCode.BadRequest);
    }
}