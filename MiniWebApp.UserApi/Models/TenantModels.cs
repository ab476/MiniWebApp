using FluentValidation;
using MiniWebApp.Core.Validator;

namespace MiniWebApp.UserApi.Models;

/// <summary>
/// Represents the public-facing tenant information returned by the API.
/// </summary>
public record TenantResponse(
    Guid Id,
    string Name,
    string? Domain,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

/// <summary>
/// Represents the data required to create a new tenant.
/// </summary>
public record CreateTenantRequest(
    string Name,
    string? Domain
);

/// <summary>
/// Represents the data required to update an existing tenant's details.
/// </summary>
public record UpdateTenantRequest(
    string Name,
    string? Domain
);

/// <summary>
/// Represents a request to activate an existing tenant.
/// </summary>
public record ActivateTenantRequest(
    Guid TenantId
);

/// <summary>
/// Represents a request to deactivate an existing tenant.
/// </summary>
public record DeactivateTenantRequest(
    Guid TenantId
);

/// <summary>
/// Validator for <see cref="UpdateTenantRequest"/>.
/// </summary>
public sealed class UpdateTenantRequestValidator
    : AbstractValidator<UpdateTenantRequest>
{
    public UpdateTenantRequestValidator()
    {
        RuleFor(x => x.Name)
            .ValidTenantName();

        RuleFor(x => x.Domain)
            .ValidDomain();
    }
}

/// <summary>
/// Validator for <see cref="ActivateTenantRequest"/>.
/// </summary>
public sealed class ActivateTenantRequestValidator
    : AbstractValidator<ActivateTenantRequest>
{
    public ActivateTenantRequestValidator()
    {
        RuleFor(x => x.TenantId)
            .ValidTenantId();
    }
}

/// <summary>
/// Shared Validation Extensions for Tenant-related properties.
/// </summary>
internal static class TenantValidationExtensions
{
    public static IRuleBuilderOptions<T, string> ValidTenantName<T>(
        this IRuleBuilder<T, string> rule)
    {
        return rule
            .NotEmpty().WithMessage("Tenant name is required.")
            .MaximumLength(200).WithMessage("Tenant name must not exceed 200 characters.")
            .IsSecurePlainText();
    }

    public static IRuleBuilderOptions<T, string?> ValidDomain<T>(
        this IRuleBuilder<T, string?> rule)
    {
        return rule
            .MaximumLength(200).WithMessage("Domain must not exceed 200 characters.") // Keep this for length validation
            .Must(SecurityInputValidator.IsAlphanumericDotHyphen)
            .When(x => x is not null)
            .WithMessage("Domain contains invalid characters.")
            .IsSecurePlainText();
    }

    public static IRuleBuilderOptions<T, Guid> ValidTenantId<T>(
        this IRuleBuilder<T, Guid> rule)
    {
        return rule
            .NotEmpty().WithMessage("TenantId must not be empty.");
    }
}

/// <summary>
/// Validator for <see cref="CreateTenantRequest"/>.
/// </summary>
public sealed class CreateTenantRequestValidator
    : AbstractValidator<CreateTenantRequest>
{
    public CreateTenantRequestValidator()
    {
        RuleFor(x => x.Name)
            .ValidTenantName();

        RuleFor(x => x.Domain)
            .ValidDomain();
    }
}

/// <summary>
/// Validator for <see cref="DeactivateTenantRequest"/>.
/// </summary>
public sealed class DeactivateTenantRequestValidator
    : AbstractValidator<DeactivateTenantRequest>
{
    public DeactivateTenantRequestValidator()
    {
        RuleFor(x => x.TenantId)
            .ValidTenantId();
    }
}