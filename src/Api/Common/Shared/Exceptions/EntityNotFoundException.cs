using System.Net;

namespace Api.Common.Shared.Exceptions;

public sealed class EntityNotFoundException<T>(Guid id) :
    ApiException(
        $"Could not find {typeof(T).Name.ToLower()}.",
        $"{typeof(T).Name} with ID '{id}' was not found.",
        $"{typeof(T).Name}.NotFound",
        HttpStatusCode.NotFound);