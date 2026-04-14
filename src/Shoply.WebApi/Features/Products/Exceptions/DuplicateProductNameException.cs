using System.Net;

namespace Shoply.WebApi.Features.Products.Exceptions;

public sealed class DuplicateProductNameException(string name) : ApiException("Duplicate Product Name",
    $"A product with the name '{name}' already exists. Please choose a different name.",
    "Product.Name.Duplicate",
    HttpStatusCode.BadRequest);