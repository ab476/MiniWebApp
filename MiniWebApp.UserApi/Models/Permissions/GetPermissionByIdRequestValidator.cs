using FluentValidation;

namespace MiniWebApp.UserApi.Models.Permissions;

public sealed class GetPermissionByIdRequestValidator
    : AbstractValidator<GetPermissionByIdRequest>
{
    public GetPermissionByIdRequestValidator()
    {
        RuleFor(x => x.Id)
            .ValidId();
    }
}
