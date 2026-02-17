using FluentValidation;

namespace MiniWebApp.UserApi.Contracts.Roles;

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









