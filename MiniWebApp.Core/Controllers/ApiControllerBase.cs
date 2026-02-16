using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using MiniWebApp.Core.Exceptions;
namespace MiniWebApp.Core.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected async Task ValidateAsync<TRequest>(TRequest request, CancellationToken ct)
    {
        var validator = HttpContext.RequestServices.GetRequiredService<IValidator<TRequest>>();

        var validationResult = await validator.ValidateAsync(request, ct);

        if (!validationResult.IsValid)
        {
            var message = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new ValidationFailedException(message);
        }
    }
}
