using FluentValidation;

namespace MiniWebApp.UserApi.Contracts.Roles;

public sealed class DeactivateRoleRequestValidator : AbstractValidator<DeactivateRoleRequest>
{
    public DeactivateRoleRequestValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmptyGuid(nameof(DeactivateRoleRequest.RoleId));
    }
}









