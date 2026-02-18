using FluentValidation;

namespace MiniWebApp.UserApi.Models.Roles;

public sealed class CreateRoleRequestValidator : AbstractValidator<CreateRoleRequest>
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









