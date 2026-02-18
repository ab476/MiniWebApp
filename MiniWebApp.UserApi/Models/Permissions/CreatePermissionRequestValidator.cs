using FluentValidation;

namespace MiniWebApp.UserApi.Models.Permissions;

// ============================================================
// VALIDATORS
// ============================================================

public sealed class CreatePermissionRequestValidator
    : AbstractValidator<CreatePermissionRequest>
{
    public CreatePermissionRequestValidator()
    {
        RuleFor(x => x.Code)
            .ValidPermissionCode();

        RuleFor(x => x.Description)
            .ValidDescription();

        RuleFor(x => x.Category)
            .ValidCategory();
    }
}
