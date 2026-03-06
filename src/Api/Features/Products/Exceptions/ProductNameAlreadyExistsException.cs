using System.Net;
using Api.Common.Shared.Exceptions;

namespace Api.Features.Products.Exceptions;

public sealed class ProductNameAlreadyExistsException(string name) : ApiException("Product Name Already Exists",
    $"A product with the name '{name}' already exists. Please choose a different name.",
    "Product.Name.AlreadyExists",
    HttpStatusCode.BadRequest);