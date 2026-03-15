using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace MiniWebApp.Core.Common;

public interface IOutcome : IConvertToActionResult
{
    bool IsSuccess { get; }
    string? Error { get; }
    int? StatusCode { get; }
}

// Base logic to handle the MVC conversion
public abstract record OutcomeBase(bool IsSuccess, string? Error, int? StatusCode) : IOutcome
{
    public IActionResult Convert() => new ObjectResult(this) { StatusCode = StatusCode };
    public Outcome<T> ToFailure<T>(string? error = null, int? statusCode = null)
    {
        return (error ?? this.Error ?? "Unknown error", statusCode ?? this.StatusCode ?? StatusCodes.Status500InternalServerError);
    }
}

/// <summary> Non-generic result </summary>
public sealed record Outcome(bool IsSuccess, string? Error = null, int? StatusCode = null)
    : OutcomeBase(IsSuccess, Error, StatusCode)
{
    public static implicit operator Outcome(int code) => new(true, null, code);
    public static implicit operator Outcome((string err, int code) t) => new(false, t.err, t.code);
}

/// <summary> Generic result with payload </summary>
public sealed record Outcome<T>(bool IsSuccess, T? Value = default, string? Error = null, int? StatusCode = null)
    : OutcomeBase(IsSuccess, Error, StatusCode)
{
    public static implicit operator Outcome<T>((int code, T val) t) => new(true, t.val, null, t.code);
    public static implicit operator Outcome<T>((string err, int code) t) => new(false, default, t.err, t.code);
}

//public static class Outcome
//{
//    public static Outcome Success(int code = StatusCodes.Status200OK) => code;
//    public static Outcome Failure(string error, int code) => (error, code);
    
//    public static Outcome<T> Success<T>(T value, int code = StatusCodes.Status200OK) => (code, value);
//    public static Outcome<T> Failure<T>(string error, int code) => (error, code);
//}

