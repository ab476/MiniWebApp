using static Microsoft.AspNetCore.Http.StatusCodes;

namespace MiniWebApp.Core.Exceptions;

/// <summary>Base exception for application-specific errors with an associated HTTP status code.</summary>
public abstract class AppException(string message, int statusCode) : Exception(message)
{
    public readonly int StatusCode = statusCode;
}

/// <summary>Thrown when a requested resource is not found.</summary>
public sealed class NotFoundException(string message)
    : AppException(message, Status404NotFound);

/// <summary>Thrown when a resource state conflict occurs (e.g., duplicate unique keys).</summary>
public sealed class ConflictException(string message)
    : AppException(message, Status409Conflict);

/// <summary>Thrown when the request payload or parameters are invalid.</summary>
public sealed class BadRequestException(string message)
    : AppException(message, Status400BadRequest);

/// <summary>Thrown when authentication is missing or invalid.</summary>
public sealed class UnauthorizedException(string message)
    : AppException(message, Status401Unauthorized);

/// <summary>Thrown when a user lacks valid credentials for the requested operation.</summary>
public sealed class UnauthorizedAccessException(string message)
    : AppException(message, Status401Unauthorized);

/// <summary>Thrown when an authenticated user lacks permission to access a resource.</summary>
public sealed class ForbiddenException(string message)
    : AppException(message, Status403Forbidden);

/// <summary>Thrown when data validation logic fails.</summary>
public sealed class ValidationFailedException(string message)
    : AppException(message, Status400BadRequest);

/// <summary>Thrown when an action is invalid given the current state of the object.</summary>
public sealed class InvalidOperationException(string message)
    : AppException(message, Status400BadRequest);

/// <summary>Thrown when an external service or dependency is unavailable.</summary>
public sealed class ServiceUnavailableException(string message)
    : AppException(message, Status503ServiceUnavailable);

/// <summary>Thrown when an unhandled internal error occurs.</summary>
public sealed class InternalServerException(string message)
    : AppException(message, Status500InternalServerError);

/// <summary>Thrown when a specific entity lookup fails using its identifier.</summary>
public sealed class EntityNotFoundException(string entityName, object key)
    : AppException(
        $"{entityName} with key '{key}' was not found.",
        Status404NotFound
    );