using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniWebApp.Core.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using FluentValidationException = FluentValidation.ValidationException;

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
                    FluentValidationException validationEx => (
                        StatusCodes.Status400BadRequest,
                        string.Join("; ", validationEx.Errors.Select(e => e.ErrorMessage))
                    ),
                    NotFoundException notFoundEx => new (
                        StatusCodes.Status404NotFound,
                        notFoundEx.Message
                    ),
                    ConflictException conflictEx => new (
                        StatusCodes.Status409Conflict,
                        conflictEx.Message
                    ),

                    _ => new (
                        StatusCodes.Status500InternalServerError,
                        "An unexpected error occurred."
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