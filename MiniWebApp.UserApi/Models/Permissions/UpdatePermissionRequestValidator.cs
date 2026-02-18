using FluentValidation;

namespace MiniWebApp.UserApi.Models.Permissions;

public sealed class UpdatePermissionRequestValidator
    : AbstractValidator<UpdatePermissionRequest>
{
    public UpdatePermissionRequestValidator()
    {
        RuleFor(x => x.Code)
            .ValidPermissionCode();

        RuleFor(x => x.Description)
            .ValidDescription();

        RuleFor(x => x.Category)
            .ValidCategory();
    }
}
