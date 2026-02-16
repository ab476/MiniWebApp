using FluentValidation;
using MiniWebApp.Core.Validator;

namespace MiniWebApp.UserApi.Contracts.Roles;

#region Contracts

public record CreateRoleRequest(
    Guid TenantId,
    string Name,
    string? Description
);

public record UpdateRoleRequest(
    string Name,
    string? Description
);

public record ActivateRoleRequest(Guid RoleId);

public record DeactivateRoleRequest(Guid RoleId);

public record RoleResponse(
    Guid Id,
    Guid TenantId,
    string Name,
    string NormalizedName,
    string? Description,
    DateTime CreatedAt
);

#endregion

#region Validation Extensions (Reusable Field Rules)

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

#endregion

#region Validators

public sealed class CreateRoleRequestValidator
    : AbstractValidator<CreateRoleRequest>
{
    public CreateRoleRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.TenantId)
            .NotEmptyGuid(nameof(CreateRoleRequest.TenantId));

        RuleFor(x => x.Name)
            .ValidRoleName();

        RuleFor(x => x.Description)
            .ValidRoleDescription();
    }
}

public sealed class UpdateRoleRequestValidator
    : AbstractValidator<UpdateRoleRequest>
{
    public UpdateRoleRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Name)
            .ValidRoleName();

        RuleFor(x => x.Description)
            .ValidRoleDescription();
    }
}

public sealed class ActivateRoleRequestValidator
    : AbstractValidator<ActivateRoleRequest>
{
    public ActivateRoleRequestValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmptyGuid(nameof(ActivateRoleRequest.RoleId));
    }
}

public sealed class DeactivateRoleRequestValidator
    : AbstractValidator<DeactivateRoleRequest>
{
    public DeactivateRoleRequestValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmptyGuid(nameof(DeactivateRoleRequest.RoleId));
    }
}

#endregion
