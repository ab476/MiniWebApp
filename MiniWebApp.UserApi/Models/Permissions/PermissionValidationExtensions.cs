using FluentValidation;

namespace MiniWebApp.UserApi.Models.Permissions;

// ============================================================
// SHARED VALIDATION EXTENSIONS
// ============================================================

internal static class PermissionValidationExtensions
{
    public static IRuleBuilderOptions<T, string> ValidPermissionCode<T>(
        this IRuleBuilder<T, string> rule)
    {
        return rule
            .NotEmpty().WithMessage("Permission code is required.")
            .MaximumLength(150).WithMessage("Permission code must not exceed 150 characters.")
            .Matches(@"^[A-Z0-9_.:-]+$")
            .WithMessage("Permission code must be uppercase and contain only letters, numbers, '.', '_', '-', ':'.");
    }

    public static IRuleBuilderOptions<T, string?> ValidDescription<T>(
        this IRuleBuilder<T, string?> rule)
    {
        return rule
            .MaximumLength(500)
            .WithMessage("Description must not exceed 500 characters.");
    }

    public static IRuleBuilderOptions<T, string?> ValidCategory<T>(
        this IRuleBuilder<T, string?> rule)
    {
        return rule
            .MaximumLength(150)
            .WithMessage("Category must not exceed 150 characters.");
    }

    public static IRuleBuilderOptions<T, Guid> ValidId<T>(
        this IRuleBuilder<T, Guid> rule)
    {
        return rule
            .NotEmpty()
            .WithMessage("Id must not be empty.");
    }
}
