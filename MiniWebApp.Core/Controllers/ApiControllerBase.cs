using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using FluentValidationException = FluentValidation.ValidationException;
namespace MiniWebApp.Core.Controllers;

[ApiController]
[Produces("application/json")]
public abstract class ApiControllerBase : ControllerBase
{
    protected async Task ValidateAsync<TRequest>(TRequest request)
    {
        var validator = HttpContext.RequestServices.GetRequiredService<IValidator<TRequest>>();

        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            throw new FluentValidationException(validationResult.Errors);
        }
    }
}
