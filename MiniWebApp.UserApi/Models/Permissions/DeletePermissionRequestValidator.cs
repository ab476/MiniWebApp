using FluentValidation;

namespace MiniWebApp.UserApi.Models.Permissions;

public sealed class DeletePermissionRequestValidator
    : AbstractValidator<DeletePermissionRequest>
{
    public DeletePermissionRequestValidator()
    {
        RuleFor(x => x.Id)
            .ValidId();
    }
}
