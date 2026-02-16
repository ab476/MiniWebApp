using static Microsoft.AspNetCore.Http.StatusCodes;

namespace MiniWebApp.Core.Exceptions;

public abstract class AppException(string message, int statusCode) : Exception(message)
{
    public readonly int StatusCode = statusCode;
}

public sealed class NotFoundException(string message)
    : AppException(message, Status404NotFound);

public sealed class ConflictException(string message)
    : AppException(message, Status409Conflict);

public sealed class BadRequestException(string message)
    : AppException(message, Status400BadRequest);

public sealed class UnauthorizedException(string message)
    : AppException(message, Status401Unauthorized);

public sealed class ForbiddenException(string message)
    : AppException(message, Status403Forbidden);

public sealed class ValidationFailedException(string message)
    : AppException(message, Status400BadRequest);

public sealed class ServiceUnavailableException(string message)
    : AppException(message, Status503ServiceUnavailable);

public sealed class InternalServerException(string message)
    : AppException(message, Status500InternalServerError);

public sealed class EntityNotFoundException(string entityName, object key)
    : AppException(
        $"{entityName} with key '{key}' was not found.",
        Status404NotFound
    );

