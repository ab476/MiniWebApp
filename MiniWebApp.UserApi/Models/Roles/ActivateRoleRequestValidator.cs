using FluentValidation;

namespace MiniWebApp.UserApi.Models.Roles;

public sealed class ActivateRoleRequestValidator : AbstractValidator<ActivateRoleRequest>
{
    public ActivateRoleRequestValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmptyGuid(nameof(ActivateRoleRequest.RoleId));
    }
}