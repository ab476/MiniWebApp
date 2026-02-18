using FluentValidation;

namespace MiniWebApp.UserApi.Models.Tenants;

public sealed class ActivateTenantRequestValidator
    : AbstractValidator<ActivateTenantRequest>
{
    public ActivateTenantRequestValidator()
    {
        RuleFor(x => x.TenantId)
            .ValidTenantId();
    }
}
