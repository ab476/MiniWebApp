using System;
using System.Collections.Generic;
using System.Text;

namespace MiniWebApp.Core.Exceptions;

public abstract class AppException(string message) : Exception(message);

public sealed class NotFoundException(string message)
    : AppException(message);

public sealed class ConflictException(string message)
    : AppException(message);

public sealed class BadRequestException(string message)
    : AppException(message);

public sealed class UnauthorizedException(string message)
    : AppException(message);

public sealed class ForbiddenException(string message)
    : AppException(message);

public sealed class ValidationFailedException(string message)
    : AppException(message);

public sealed class ServiceUnavailableException(string message)
    : AppException(message);

public sealed class InternalServerException(string message)
    : AppException(message);

public sealed class EntityNotFoundException(string entityName, object key)
    : AppException($"{entityName} with key '{key}' was not found.");
