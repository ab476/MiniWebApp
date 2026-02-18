using FluentValidation;
using MiniWebApp.Core.Validator;

namespace MiniWebApp.UserApi.Models.Roles;

internal static class RoleValidationExtensions
{
    public static IRuleBuilderOptions<T, Guid> NotEmptyGuid<T>(
        this IRuleBuilder<T, Guid> rule,
        string fieldName)
    {
        return rule
            .NotEmpty()
            .WithMessage($"{fieldName} is required.");
    }

    public static IRuleBuilderOptions<T, string> ValidRoleName<T>(
        this IRuleBuilder<T, string> rule)
    {
        return rule
            .NotEmpty().WithMessage("Role name is required.")
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .WithMessage("Role name cannot be whitespace.")
            .MaximumLength(100)
            .WithMessage("Role name cannot exceed 100 characters.")
            .NoMaliciousContent();
    }

    public static IRuleBuilderOptions<T, string?> ValidRoleDescription<T>(
        this IRuleBuilder<T, string?> rule)
    {
        return rule
            .MaximumLength(500)
            .WithMessage("Description cannot exceed 500 characters.")
            .NoMaliciousContent();
    }
}









