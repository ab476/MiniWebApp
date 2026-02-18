using FluentValidation;

namespace MiniWebApp.UserApi.Models.Roles;

public sealed class DeactivateRoleRequestValidator : AbstractValidator<DeactivateRoleRequest>
{
    public DeactivateRoleRequestValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmptyGuid(nameof(DeactivateRoleRequest.RoleId));
    }
}









