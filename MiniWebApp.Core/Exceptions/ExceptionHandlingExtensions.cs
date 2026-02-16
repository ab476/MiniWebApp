using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using MiniWebApp.Core.Common;
using System.Text.Json.Serialization;

namespace MiniWebApp.Core.Exceptions;

public static class ExceptionHandlingExtensions
{
    public static void UseExceptionHandlingMiddleware(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
                Outcome<string> problem = exception switch
                {
                    AppException applicationEx => (applicationEx.Message, applicationEx.StatusCode),
                    _ => new(
                        "An unexpected error occurred.",
                        StatusCodes.Status500InternalServerError
                    )
                };
            

                context.Response.StatusCode = problem.StatusCode ?? StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(problem, ApiJsonContext.Default.OutcomeString);
            });
        });
    }
}

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(Outcome<string>))]
internal partial class ApiJsonContext : JsonSerializerContext
{
}