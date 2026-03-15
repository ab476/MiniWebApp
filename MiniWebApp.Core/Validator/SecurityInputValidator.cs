using FluentValidation;
using System.Text.RegularExpressions;

namespace MiniWebApp.Core.Validator;

public static partial class SecurityInputValidator
{
    // 1. Matches < or > (HTML tags)
    // 2. Matches ; or -- or /* (SQL command chaining/comments)
    // Generated as a source-generated Regex for maximum performance
    [GeneratedRegex(@"[<>]|;|--|/\*", RegexOptions.Compiled)]
    private static partial Regex IllegalCharactersRegex();

    // Matches lowercase alphanumeric, dots, and underscores
    [GeneratedRegex(@"^[a-z0-9._]+$", RegexOptions.Compiled)]
    private static partial Regex NaturalKeyRegex();

    // Matches alphanumeric, dots, and hyphens for domain names
    [GeneratedRegex(@"^[a-zA-Z0-9.-]+$", RegexOptions.Compiled)]
    private static partial Regex DomainRegex();

    public static bool IsPlainTextSecure(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return true;

        // Returns true if NO illegal characters are found
        return !IllegalCharactersRegex().IsMatch(input);
    }
    public static bool IsValidNaturalKey(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;
        return NaturalKeyRegex().IsMatch(input);
    }

    /// <summary>
    /// Checks if the input string contains only alphanumeric characters, dots, and hyphens.
    /// </summary>
    /// <param name="input">The string to validate.</param>
    /// <returns>True if the string is valid or null/empty, false otherwise.</returns>
    public static bool IsAlphanumericDotHyphen(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return true; // Null or empty domains are considered valid (optional)
        return DomainRegex().IsMatch(input);
    }
}

public static class SecurityValidationExtensions
{
    public static IRuleBuilderOptions<T, string> IsSecurePlainText<T>(
        this IRuleBuilder<T, string> rule)
    {
        return rule
            .Must(SecurityInputValidator.IsPlainTextSecure)
            .WithMessage("{PropertyName} contains illegal characters (<, >, ;, --, or /*).");
    }
    public static IRuleBuilderOptions<T, string> IsNaturalKey<T>(
        this IRuleBuilder<T, string> rule)
    {
        return rule
            .Must(SecurityInputValidator.IsValidNaturalKey)
            .WithMessage("{PropertyName} must be lowercase alphanumeric with dots or underscores (e.g., 'user.edit_all').");
    }
}