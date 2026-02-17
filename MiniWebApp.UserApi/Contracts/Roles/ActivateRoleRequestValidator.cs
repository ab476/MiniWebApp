using FluentValidation;

namespace MiniWebApp.UserApi.Contracts.Roles;

public sealed class ActivateRoleRequestValidator : AbstractValidator<ActivateRoleRequest>
{
    public ActivateRoleRequestValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmptyGuid(nameof(ActivateRoleRequest.RoleId));
    }
}