using FluentValidation;

namespace MiniWebApp.UserApi.Models.Tenants;

public sealed class DeactivateTenantRequestValidator
    : AbstractValidator<DeactivateTenantRequest>
{
    public DeactivateTenantRequestValidator()
    {
        RuleFor(x => x.TenantId)
            .ValidTenantId();
    }
}
