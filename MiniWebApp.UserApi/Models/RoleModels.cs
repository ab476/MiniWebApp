using FluentValidation;
using MiniWebApp.Core.Validator;
using System.Linq.Expressions;

namespace MiniWebApp.UserApi.Models;

/// <summary>
/// Represents a request to activate an existing role.
/// </summary>
/// <param name="RoleCode">The unique identifier for the role to be activated.</param>
public record ActivateRoleRequest(string RoleCode);

/// <summary>
/// Represents a request to deactivate an existing role, preventing users from being assigned to it.
/// </summary>
/// <param name="RoleCode">The unique identifier for the role to be deactivated.</param>
public record DeactivateRoleRequest(string RoleCode);

/// <summary>
/// Represents the data required to create a new security role.
/// </summary>
/// <param name="RoleCode">The unique, normalized code for the role (e.g., "SUPER_ADMIN").</param>
/// <param name="DisplayName">The human-readable display name of the role.</param>
/// <param name="TenantId">The unique identifier of the tenant this role belongs to. Defaults to null for global roles.</param>
public record CreateRoleRequest(
    string RoleCode,
    string DisplayName,
    Guid? TenantId = null);

/// <summary>
/// Represents the data required to update an existing security role's details.
/// </summary>
/// <param name="RoleCode">The unique code of the role being updated.</param>
/// <param name="DisplayName">The new human-readable display name for the role.</param>
/// <param name="TenantId">The unique identifier of the tenant this role belongs to.</param>
public record UpdateRoleRequest(
    string RoleCode,
    string DisplayName,
    Guid? TenantId = null);

/// <summary>
/// Represents a request to delete a specific role from the system.
/// </summary>
/// <param name="RoleCode">The unique identifier code of the role to be deleted.</param>
/// <param name="TenantId">
/// The unique identifier of the tenant. If <see langword="null"/>, the system 
/// will typically resolve the tenant from the current execution context.
/// </param>
public record DeleteRoleRequest(
    string RoleCode,
    Guid? TenantId = null);

/// <summary>
/// Represents the public-facing role information returned by the API.
/// </summary>
/// <param name="RoleCode">The unique, normalized code for the role.</param>
/// <param name="TenantId">The owner tenant's unique identifier.</param>
/// <param name="DisplayName">The display name of the role.</param>
/// <param name="CreatedAt">The date and time (UTC) when the role was originally created.</param>
public record RoleResponse(
    string RoleCode,
    Guid TenantId,
    string? DisplayName,
    DateTime CreatedAt);
public sealed class CreateRoleRequestValidator : AbstractValidator<CreateRoleRequest>
{
    public CreateRoleRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.RoleCode).ValidRoleCode();
        RuleFor(x => x.DisplayName).ValidRoleName();
        //RuleFor(x => x.Description).ValidRoleDescription();
    }
}

public sealed class UpdateRoleRequestValidator : AbstractValidator<UpdateRoleRequest>
{
    public UpdateRoleRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        // RoleCode is the immutable key; Name and Description are updatable
        RuleFor(x => x.RoleCode).ValidRoleCode();
        RuleFor(x => x.DisplayName).ValidRoleName();
        //RuleFor(x => x.Description).ValidRoleDescription();
    }
}

public sealed class ActivateRoleRequestValidator : AbstractValidator<ActivateRoleRequest>
{
    public ActivateRoleRequestValidator() => RuleFor(x => x.RoleCode).ValidRoleCode();
}
internal static class RoleValidationExtensions
{
    public static IRuleBuilderOptions<T, string> ValidRoleCode<T>(
        this IRuleBuilder<T, string> rule)
    {
        return rule
            .NotEmpty().WithMessage("Role code is required.")
            .MaximumLength(50)
            .IsNaturalKey()
            .IsSecurePlainText();
    }

    public static IRuleBuilderOptions<T, string> ValidRoleName<T>(
        this IRuleBuilder<T, string> rule)
    {
        return rule
            .NotEmpty().WithMessage("Role name is required.")
            .MaximumLength(100)
            .IsSecurePlainText();
    }

    public static IRuleBuilderOptions<T, string?> ValidRoleDescription<T>(
        this IRuleBuilder<T, string?> rule)
    {
        return rule
            .MaximumLength(500)
            .IsSecurePlainText();
    }
}

public static partial class RoleMapperExtensions
{
    private static readonly Expression<Func<Role, RoleResponse>> MappingExpression =
        role => new RoleResponse(
            role.RoleCode,
            role.TenantId,
            role.DisplayName,
            role.CreatedAt
        );

    private static readonly Func<Role, RoleResponse> CompiledMapper =
        MappingExpression.Compile();

    /// <summary>  Projects an IQueryable directly to RoleResponse (EF Core optimized).  </summary>
    public static IQueryable<RoleResponse> ProjectToResponse(this IQueryable<Role> query)
    {
        return query.Select(MappingExpression);
    }

    /// <summary>  Converts a single Role entity to a RoleResponse.  </summary>
    public static RoleResponse ToResponse(this Role role)
    {
        return CompiledMapper.Invoke(role);
    }
}