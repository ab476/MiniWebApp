using FluentValidation;

namespace MiniWebApp.UserApi.Models.Permissions;

public sealed class GetPermissionRequestValidator
    : AbstractValidator<GetPermissionRequest>
{
    public GetPermissionRequestValidator()
    {
        // 1. Force the 'at least one' rule at the object level
        RuleFor(x => x)
            .Must(x => x.Id != Guid.Empty || !string.IsNullOrWhiteSpace(x.Code))
            .WithMessage("You must provide either a Permission ID or a Code.");

        // 2. Specific validation for Id (only if it's actually provided)
        RuleFor(x => x.Id)
            .NotEmpty()
            .When(x => x.Id != null && x.Id != Guid.Empty)
            .WithMessage("The provided ID is not a valid GUID.");

        // 3. Specific validation for Code (only if it's actually provided)
        RuleFor(x => x.Code)
            .MaximumLength(150)
            .When(x => !string.IsNullOrWhiteSpace(x.Code))
            .WithMessage("The Permission Code is too long.");
    }
}
