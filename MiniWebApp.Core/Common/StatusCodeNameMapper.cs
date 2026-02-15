using Microsoft.AspNetCore.Http;

namespace MiniWebApp.Core.Common;

public static class StatusCodeNameMapper
{
    public static string ToStatusName(this int? statusCode) =>
        statusCode switch
        {
            100 => nameof(StatusCodes.Status100Continue),
            101 => nameof(StatusCodes.Status101SwitchingProtocols),
            102 => nameof(StatusCodes.Status102Processing),

            200 => nameof(StatusCodes.Status200OK),
            201 => nameof(StatusCodes.Status201Created),
            202 => nameof(StatusCodes.Status202Accepted),
            203 => nameof(StatusCodes.Status203NonAuthoritative),
            204 => nameof(StatusCodes.Status204NoContent),
            205 => nameof(StatusCodes.Status205ResetContent),
            206 => nameof(StatusCodes.Status206PartialContent),
            207 => nameof(StatusCodes.Status207MultiStatus),
            208 => nameof(StatusCodes.Status208AlreadyReported),
            226 => nameof(StatusCodes.Status226IMUsed),

            300 => nameof(StatusCodes.Status300MultipleChoices),
            301 => nameof(StatusCodes.Status301MovedPermanently),
            302 => nameof(StatusCodes.Status302Found),
            303 => nameof(StatusCodes.Status303SeeOther),
            304 => nameof(StatusCodes.Status304NotModified),
            305 => nameof(StatusCodes.Status305UseProxy),
            306 => nameof(StatusCodes.Status306SwitchProxy),
            307 => nameof(StatusCodes.Status307TemporaryRedirect),
            308 => nameof(StatusCodes.Status308PermanentRedirect),

            400 => nameof(StatusCodes.Status400BadRequest),
            401 => nameof(StatusCodes.Status401Unauthorized),
            402 => nameof(StatusCodes.Status402PaymentRequired),
            403 => nameof(StatusCodes.Status403Forbidden),
            404 => nameof(StatusCodes.Status404NotFound),
            405 => nameof(StatusCodes.Status405MethodNotAllowed),
            406 => nameof(StatusCodes.Status406NotAcceptable),
            407 => nameof(StatusCodes.Status407ProxyAuthenticationRequired),
            408 => nameof(StatusCodes.Status408RequestTimeout),
            409 => nameof(StatusCodes.Status409Conflict),
            410 => nameof(StatusCodes.Status410Gone),
            411 => nameof(StatusCodes.Status411LengthRequired),
            412 => nameof(StatusCodes.Status412PreconditionFailed),
            413 => nameof(StatusCodes.Status413PayloadTooLarge),
            414 => nameof(StatusCodes.Status414UriTooLong),
            415 => nameof(StatusCodes.Status415UnsupportedMediaType),
            416 => nameof(StatusCodes.Status416RangeNotSatisfiable),
            417 => nameof(StatusCodes.Status417ExpectationFailed),
            418 => nameof(StatusCodes.Status418ImATeapot),
            419 => nameof(StatusCodes.Status419AuthenticationTimeout),
            421 => nameof(StatusCodes.Status421MisdirectedRequest),
            422 => nameof(StatusCodes.Status422UnprocessableEntity),
            423 => nameof(StatusCodes.Status423Locked),
            424 => nameof(StatusCodes.Status424FailedDependency),
            426 => nameof(StatusCodes.Status426UpgradeRequired),
            428 => nameof(StatusCodes.Status428PreconditionRequired),
            429 => nameof(StatusCodes.Status429TooManyRequests),
            431 => nameof(StatusCodes.Status431RequestHeaderFieldsTooLarge),
            451 => nameof(StatusCodes.Status451UnavailableForLegalReasons),
            499 => nameof(StatusCodes.Status499ClientClosedRequest),

            500 => nameof(StatusCodes.Status500InternalServerError),
            501 => nameof(StatusCodes.Status501NotImplemented),
            502 => nameof(StatusCodes.Status502BadGateway),
            503 => nameof(StatusCodes.Status503ServiceUnavailable),
            504 => nameof(StatusCodes.Status504GatewayTimeout),
            505 => nameof(StatusCodes.Status505HttpVersionNotsupported),
            506 => nameof(StatusCodes.Status506VariantAlsoNegotiates),
            507 => nameof(StatusCodes.Status507InsufficientStorage),
            508 => nameof(StatusCodes.Status508LoopDetected),
            510 => nameof(StatusCodes.Status510NotExtended),
            511 => nameof(StatusCodes.Status511NetworkAuthenticationRequired),

            _ => $"Status {(statusCode?.ToString() ?? string.Empty)} Unknown"
        };

    private static readonly Dictionary<string, int> _map =
       new(StringComparer.Ordinal)
       {
           ["Status100Continue"] = 100,
           ["Status101SwitchingProtocols"] = 101,
           ["Status102Processing"] = 102,

           ["Status200OK"] = 200,
           ["Status201Created"] = 201,
           ["Status202Accepted"] = 202,
           ["Status203NonAuthoritative"] = 203,
           ["Status204NoContent"] = 204,
           ["Status205ResetContent"] = 205,
           ["Status206PartialContent"] = 206,
           ["Status207MultiStatus"] = 207,
           ["Status208AlreadyReported"] = 208,
           ["Status226IMUsed"] = 226,

           ["Status300MultipleChoices"] = 300,
           ["Status301MovedPermanently"] = 301,
           ["Status302Found"] = 302,
           ["Status303SeeOther"] = 303,
           ["Status304NotModified"] = 304,
           ["Status305UseProxy"] = 305,
           ["Status306SwitchProxy"] = 306,
           ["Status307TemporaryRedirect"] = 307,
           ["Status308PermanentRedirect"] = 308,

           ["Status400BadRequest"] = 400,
           ["Status401Unauthorized"] = 401,
           ["Status402PaymentRequired"] = 402,
           ["Status403Forbidden"] = 403,
           ["Status404NotFound"] = 404,
           ["Status405MethodNotAllowed"] = 405,
           ["Status406NotAcceptable"] = 406,
           ["Status407ProxyAuthenticationRequired"] = 407,
           ["Status408RequestTimeout"] = 408,
           ["Status409Conflict"] = 409,
           ["Status410Gone"] = 410,
           ["Status411LengthRequired"] = 411,
           ["Status412PreconditionFailed"] = 412,
           ["Status413PayloadTooLarge"] = 413,
           ["Status414UriTooLong"] = 414,
           ["Status415UnsupportedMediaType"] = 415,
           ["Status416RangeNotSatisfiable"] = 416,
           ["Status417ExpectationFailed"] = 417,
           ["Status418ImATeapot"] = 418,
           ["Status419AuthenticationTimeout"] = 419,
           ["Status421MisdirectedRequest"] = 421,
           ["Status422UnprocessableEntity"] = 422,
           ["Status423Locked"] = 423,
           ["Status424FailedDependency"] = 424,
           ["Status426UpgradeRequired"] = 426,
           ["Status428PreconditionRequired"] = 428,
           ["Status429TooManyRequests"] = 429,
           ["Status431RequestHeaderFieldsTooLarge"] = 431,
           ["Status451UnavailableForLegalReasons"] = 451,
           ["Status499ClientClosedRequest"] = 499,

           ["Status500InternalServerError"] = 500,
           ["Status501NotImplemented"] = 501,
           ["Status502BadGateway"] = 502,
           ["Status503ServiceUnavailable"] = 503,
           ["Status504GatewayTimeout"] = 504,
           ["Status505HttpVersionNotsupported"] = 505,
           ["Status506VariantAlsoNegotiates"] = 506,
           ["Status507InsufficientStorage"] = 507,
           ["Status508LoopDetected"] = 508,
           ["Status510NotExtended"] = 510,
           ["Status511NetworkAuthenticationRequired"] = 511,
       };

    /// <summary>
    ///  🔒 Strict version (throws)
    /// </summary>
    /// <param name="statusName"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="KeyNotFoundException"></exception>
    public static int ToStatusCode(this string statusName)
    {
        if (string.IsNullOrWhiteSpace(statusName))
            throw new ArgumentException("Status name cannot be null or empty.", nameof(statusName));

        var normalized = Normalize(statusName);

        if (_map.TryGetValue(normalized, out var code))
            return code;

        throw new KeyNotFoundException(
            $"Invalid HTTP status name '{statusName}'.");
    }

    /// <summary>
    /// ✅ Safe version (returns null if not found)
    /// </summary>
    /// <param name="statusName"></param>
    /// <returns></returns>
    public static int? TryToStatusCode(this string? statusName)
    {
        if (string.IsNullOrWhiteSpace(statusName))
            return null;

        var normalized = Normalize(statusName);

        return _map.TryGetValue(normalized, out var code)
            ? code
            : null;
    }

    private static string Normalize(string statusName)
    {
        return statusName.StartsWith("Status", StringComparison.Ordinal)
            ? statusName
            : $"Status{statusName}";
    }
}

