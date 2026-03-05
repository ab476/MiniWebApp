using FluentValidation;
using MiniWebApp.Core.Security;

namespace MiniWebApp.UserApi.Models.Roles;

public sealed class CreateRoleRequestValidator : AbstractValidator<CreateRoleRequest>
{
    public CreateRoleRequestValidator(IUserContext userContext)
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.TenantId)
            .NotEmptyGuid(nameof(CreateRoleRequest.TenantId))
            .Must(tenantId => tenantId == userContext.TenantId || userContext.IsSuperAdmin)
            .WithMessage("Access Denied: You do not have permission to access the requested resource.");

        RuleFor(x => x.Name)
            .ValidRoleName();

        RuleFor(x => x.Description)
            .ValidRoleDescription();
    }
}









