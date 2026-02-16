using FluentValidation;
using Ganss.Xss;

namespace MiniWebApp.Core.Validator;

internal static class SecurityInputValidator
{
    private static readonly HtmlSanitizer _sanitizer = new();

    public static bool ContainsMaliciousHtml(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return false;

        var sanitized = _sanitizer.Sanitize(input);

        // If sanitization changes the string → malicious content existed
        return !string.Equals(input, sanitized, StringComparison.Ordinal);
    }

    public static bool ContainsSqlMetaCharacters(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return false;

        string[] dangerousPatterns =
        {
            "--", ";--", ";", "/*", "*/",
            "@@", "char(", "nchar(", "varchar(",
            "alter ", "begin ", "cast(", "create ",
            "cursor ", "declare ", "delete ",
            "drop ", "end ", "exec ", "execute ",
            "insert ", "kill ", "select ",
            "sys.", "sysobjects", "syscolumns",
            "update "
        };

        return dangerousPatterns.Any(p =>
            input.Contains(p, StringComparison.OrdinalIgnoreCase));
    }
}
public static class SecurityValidationExtensions
{

    public static IRuleBuilderOptions<T, string> NoMaliciousContent<T>(
    this IRuleBuilder<T, string> rule)
    {
        return rule.Must(input =>
            !SecurityInputValidator.ContainsMaliciousHtml(input))
        .WithMessage("Input contains potentially malicious content.");
    }

    public static IRuleBuilderOptions<T, string?> NoMaliciousContentNullable<T>(
        this IRuleBuilder<T, string?> rule)
    {
        return rule.Must(input =>
            string.IsNullOrWhiteSpace(input) ||
            !SecurityInputValidator.ContainsMaliciousHtml(input))
        .WithMessage("Input contains potentially malicious content.");
    }

}
