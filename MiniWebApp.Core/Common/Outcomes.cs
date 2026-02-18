using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace MiniWebApp.Core.Common;

public interface IOutcome : IConvertToActionResult
{
    bool IsSuccess { get; }
    string? Error { get; }
    int? StatusCode { get; }
}

/// <summary>
/// Represents the result of a non-generic application operation
/// with optional HTTP semantics (no response payload).
///
/// Supports implicit conversions:
///
/// Success:
/// <code>
/// Outcome result = StatusCodes.Status204NoContent;
/// </code>
/// 
/// Failure:
/// <code>
/// Outcome result = (StatusCodes.Status400BadRequest, "Validation failed");
/// </code>
/// 
/// Implicit conversion rules:
/// - (int statusCode) → Success
/// - (int statusCode, string error) → Failure
///
/// This enables concise service-layer returns while remaining
/// decoupled from ASP.NET MVC types:
/// <code>
/// return StatusCodes.Status204NoContent;
/// return (StatusCodes.Status404NotFound, "Permission not found.");
/// </code>
/// </summary>
public sealed class Outcome(bool isSuccess, string? error, int? statusCode) : IOutcome, IConvertToActionResult
{
    public bool IsSuccess { get; } = isSuccess;
    public string? Error { get; } = error;

    public int? StatusCode { get;  } = statusCode;

    // -------------------------------
    // Implicit Conversions
    // -------------------------------

    // (statusCode) -> Success
    public static implicit operator Outcome(int statusCode)
    {
        return new(true, null, statusCode);
    }

    // (statusCode, errorMessage) -> Failure
    public static implicit operator Outcome((string error, int statusCode) tuple)
    {
        return new(false, tuple.error, tuple.statusCode);
    }

    public IActionResult Convert()
    {
        return new ObjectResult(this)
        {
            StatusCode = StatusCode
        };
    }
}

public interface IOutcome<out T> : IOutcome
{
    T? Value { get; }
}

/// <summary>
/// Represents the result of an application operation with optional HTTP semantics.
///
/// Supports implicit conversions:
/// 
/// Success:
/// <code>
/// Outcome&lt;T&gt; result = (StatusCodes.Status200OK, value);
/// </code>
/// 
/// Failure:
/// <code>
/// Outcome&lt;T&gt; result = ("Error message", StatusCodes.Status400BadRequest);
/// </code>
/// 
/// Implicit conversion rules:
/// - (int statusCode, T value) → Success
/// - (string error, int statusCode) → Failure
///
/// This allows concise service-layer returns without coupling to MVC:
/// <code>
/// return (StatusCodes.Status201Created, response);
/// return ("Permission not found.", StatusCodes.Status404NotFound);
/// </code>
/// </summary>
public sealed class Outcome<T>(bool isSuccess, T? value, string? error, int? statusCode) : IOutcome<T>, IConvertToActionResult
{
    public bool IsSuccess { get; } = isSuccess;
    public string? Error { get; } = error;
    public int? StatusCode { get;  } = statusCode; 
    public T? Value { get; } = value;



    // --------------------------------
    // ✅ Implicit Conversions
    // --------------------------------

    /// <summary>
    /// (statusCode, data) -> Success
    /// </summary>
    /// <param name="tuple"></param>
    public static implicit operator Outcome<T>((int statusCode, T value) tuple)
    {
        return new(true, tuple.value, null, tuple.statusCode);
    }

    /// <summary>
    /// (statusCode, errorMessage) -> Failure
    /// </summary>
    /// <param name="tuple"></param>
    public static implicit operator Outcome<T>((string error, int statusCode) tuple)
    {
        return new(false, default, tuple.error, tuple.statusCode);
    }

    
    public IActionResult Convert()
    {
        return new ObjectResult(this)
        {
            StatusCode = StatusCode
        };
    }
}
