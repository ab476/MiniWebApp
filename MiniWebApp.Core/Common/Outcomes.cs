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
public abstract class OutcomeBase(bool isSuccess, string? error, int? statusCode) : IOutcome
{
    public bool IsSuccess { get; protected set; } = isSuccess;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Error { get; protected set; } = error;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Errors { get; protected set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? StatusCode { get; protected set; } = statusCode;

    public IActionResult Convert()
    {
        var code = StatusCode ?? (IsSuccess ? StatusCodes.Status200OK : StatusCodes.Status500InternalServerError);
        return new ObjectResult(this) { StatusCode = code };
    }
    public Outcome<T> ToFailure<T>(string? error = null, int? statusCode = null)
    {
        return new Outcome<T>(
            false,
            default,
            error ?? this.Error ?? "Unknown error",
            statusCode ?? this.StatusCode ?? StatusCodes.Status500InternalServerError
        );
    }
}

/// <summary> Non-generic result </summary>
public sealed class Outcome(
    bool isSuccess,
    string? error = null,
    int? statusCode = null) : OutcomeBase(isSuccess, error, statusCode)
{

    // Implicit Operators
    public static implicit operator Outcome(int code) => new(true, null, code);
    public static implicit operator Outcome((string err, int code) t) => new(false, t.err, t.code);

    // Static Factories
    public static Outcome Success(int statusCode = StatusCodes.Status200OK)
        => new(isSuccess: true, error: null, statusCode: statusCode);
    public static Outcome<T> Success<T>(T value) => Success(StatusCodes.Status200OK, value);
    public static Outcome<T> Success<T>(int statusCode, T value)
        => new(isSuccess: true, value: value, error: null, statusCode: statusCode);

    public static Outcome Failure(string error, int statusCode)
        => new(isSuccess: false, error: error, statusCode: statusCode);

    public static Outcome BadRequest(string error)
        => new(isSuccess: false, error: error, statusCode: StatusCodes.Status400BadRequest);
}

/// <summary> Generic result with payload </summary>
public sealed class Outcome<T>(
    bool isSuccess,
    T? value = default,
    string? error = null,
    int? statusCode = null) : OutcomeBase(isSuccess, error, statusCode)
{
    public T? Value { get; private set; } = value;

    // Implicit Operators
    public static implicit operator Outcome<T>((int code, T val) t) => new(true, t.val, null, t.code);
    public static implicit operator Outcome<T>((string err, int code) t) => new(false, default, t.err, t.code);

    public static Outcome<T> Success(T value, int statusCode = StatusCodes.Status200OK)
        => new(isSuccess: true, value: value, error: null, statusCode: statusCode);

    public static Outcome<T> Failure(int statusCode, string error)
        => new(isSuccess: false, value: default, error: error, statusCode: statusCode);

    // Map a non-generic failure to a generic one
    public static Outcome<T> FromFailure(IOutcome failure)
        => new(isSuccess: false, value: default, error: failure.Error, statusCode: failure.StatusCode);

    public static implicit operator Outcome<T>(Outcome failure)
        => FromFailure(failure);
}
//// Base logic to handle the MVC conversion

//public abstract record OutcomeBase(bool IsSuccess, string? Error, int? StatusCode) : IOutcome
//{
//    public IActionResult Convert() => new ObjectResult(this) { StatusCode = StatusCode };
//    public Outcome<T> ToFailure<T>(string? error = null, int? statusCode = null)
//    {
//        return (error ?? this.Error ?? "Unknown error", statusCode ?? this.StatusCode ?? StatusCodes.Status500InternalServerError);
//    }
//}

///// <summary> Non-generic result </summary>
//public sealed record Outcome(bool IsSuccess, string? Error = null, int? StatusCode = null)
//    : OutcomeBase(IsSuccess, Error, StatusCode)
//{
//    public static implicit operator Outcome(int code) => new(true, null, code);
//    public static implicit operator Outcome((string err, int code) t) => new(false, t.err, t.code);
//}

///// <summary> Generic result with payload </summary>
//public sealed record Outcome<T>(bool IsSuccess, T? Value = default, [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? Error = null, int? StatusCode = null)
//    : OutcomeBase(IsSuccess, Error, StatusCode)
//{
//    public static implicit operator Outcome<T>((int code, T val) t) => new(true, t.val, null, t.code);
//    public static implicit operator Outcome<T>((string err, int code) t) => new(false, default, t.err, t.code);
//}

//public static class Outcome
//{
//    public static Outcome Success(int code = StatusCodes.Status200OK) => code;
//    public static Outcome Failure(string error, int code) => (error, code);
    
//    public static Outcome<T> Success<T>(T value, int code = StatusCodes.Status200OK) => (code, value);
//    public static Outcome<T> Failure<T>(string error, int code) => (error, code);
//}

