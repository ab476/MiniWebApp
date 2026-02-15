using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Text.Json.Serialization;

namespace MiniWebApp.Core.Common;

public interface IOutcome : IConvertToActionResult
{
    bool IsSuccess { get; }
    string? Error { get; }
    int? StatusCode { get; }
}

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
    public static implicit operator Outcome((int statusCode, string error) tuple)
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
