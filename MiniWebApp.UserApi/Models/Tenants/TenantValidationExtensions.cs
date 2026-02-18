using FluentValidation;
using MiniWebApp.Core.Validator;

namespace MiniWebApp.UserApi.Models.Tenants;

/// <summary>
/// Shared Validation Extensions (Reusable Modules)
/// </summary>
internal static class TenantValidationExtensions
{
    public static IRuleBuilderOptions<T, string> ValidTenantName<T>(
        this IRuleBuilder<T, string> rule)
    {
        return rule
            .NotEmpty().WithMessage("Tenant name is required.")
            .MaximumLength(200).WithMessage("Tenant name must not exceed 200 characters.")
            .NoMaliciousContent();
    }

    public static IRuleBuilderOptions<T, string?> ValidDomain<T>(
        this IRuleBuilder<T, string?> rule)
    {
        return rule
            .MaximumLength(200).WithMessage("Domain must not exceed 200 characters.")
            .Matches(@"^[a-zA-Z0-9.-]+$")
            .When(x => x is not null)
            .WithMessage("Domain contains invalid characters.")
            .NoMaliciousContent();
    }

    public static IRuleBuilderOptions<T, Guid> ValidTenantId<T>(
        this IRuleBuilder<T, Guid> rule)
    {
        return rule
            .NotEmpty().WithMessage("TenantId must not be empty.");
    }
}
