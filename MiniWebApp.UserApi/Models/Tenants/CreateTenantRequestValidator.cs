using FluentValidation;

namespace MiniWebApp.UserApi.Models.Tenants;

public sealed class CreateTenantRequestValidator
    : AbstractValidator<CreateTenantRequest>
{
    public CreateTenantRequestValidator()
    {
        RuleFor(x => x.Name)
            .ValidTenantName();

        RuleFor(x => x.Domain)
            .ValidDomain();
    }
}
