using System.Net;

namespace Shoply.WebApi.Common.Shared.Exceptions;

public sealed class EntityNotFoundException<T>(Guid id) :
    ShoplyException(
        $"Could not find {typeof(T).Name.ToLower()}.",
        $"{typeof(T).Name} with ID '{id}' was not found.",
        $"{typeof(T).Name}.NotFound",
        HttpStatusCode.NotFound);