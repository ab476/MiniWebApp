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