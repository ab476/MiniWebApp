using FluentValidation;
using MiniWebApp.Core.Validator;
using System.Linq.Expressions;

namespace MiniWebApp.UserApi.Models;

public sealed record GetClaimRequest(string ClaimCode);
public sealed record ClaimResponse(
    string ClaimCode,
    string? Description,
    string Category
);

public sealed class GetClaimRequestValidator : AbstractValidator<GetClaimRequest>
{
    public GetClaimRequestValidator()
    {
        RuleFor(x => x.ClaimCode)
            .NotEmpty()
            .WithMessage("Claim code is required.")
            .MaximumLength(50)
            .WithMessage("Claim code must not exceed 50 characters.")
            .Matches(@"^[a-z0-9._]+$")
            .WithMessage("Claim code must be lowercase and can only contain letters, numbers, dots, or underscores.")
            .IsSecurePlainText();
    }
}

public static partial class ClaimMapperExtensions
{
    private static readonly Expression<Func<AppClaim, ClaimResponse>> MappingExpression =
        claim => new ClaimResponse(
            claim.ClaimCode,
            claim.Description,
            claim.Category
        );

    private static readonly Func<AppClaim, ClaimResponse> CompiledMapper =
        MappingExpression.Compile();

    /// <summary> Projects an IQueryable directly to ClaimResponse (EF Core optimized). </summary>
    public static IQueryable<ClaimResponse> ProjectToResponse(this IQueryable<AppClaim> query)
    {
        return query.Select(MappingExpression);
    }

    /// <summary> Converts a single AppClaim entity to a ClaimResponse. </summary>
    public static ClaimResponse ToResponse(this AppClaim claim)
    {
        return CompiledMapper.Invoke(claim);
    }
}