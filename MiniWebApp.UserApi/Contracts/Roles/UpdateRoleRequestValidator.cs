using FluentValidation;

namespace MiniWebApp.UserApi.Contracts.Roles;

public sealed class UpdateRoleRequestValidator : AbstractValidator<UpdateRoleRequest>
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









