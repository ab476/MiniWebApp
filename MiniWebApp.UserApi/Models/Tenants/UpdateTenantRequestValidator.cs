using FluentValidation;

namespace MiniWebApp.UserApi.Models.Tenants;

public sealed class UpdateTenantRequestValidator
    : AbstractValidator<UpdateTenantRequest>
{
    public UpdateTenantRequestValidator()
    {
        RuleFor(x => x.Name)
            .ValidTenantName();

        RuleFor(x => x.Domain)
            .ValidDomain();
    }
}
