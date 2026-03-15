

namespace MiniWebApp.UserApi.Test.Builders.Claims;

public partial class GetClaimRequestBuilder : IBuilder<GetClaimRequest, GetClaimRequestBuilder>
{
    private string _claimCode = default!;

    public static GetClaimRequestBuilder New() => new();

    public static GetClaimRequestBuilder Default => New().WithDefaults();

    public GetClaimRequestBuilder WithCode(string code)
    {
        _claimCode = code;
        return this;
    }

    /// <summary>
    /// Generates a randomized Claim Code (e.g., "claim.7a2b9c").
    /// </summary>
    public GetClaimRequestBuilder WithRandomCode()
    {
        return WithCode($"claim.{Guid.NewGuid().ToString()[..8].ToLowerInvariant()}");
    }

    public GetClaimRequestBuilder WithDefaults()
    {
        return WithRandomCode();
    }

    public GetClaimRequest Build()
    {
        return new GetClaimRequest(_claimCode);
    }

    public static implicit operator GetClaimRequest(GetClaimRequestBuilder builder) => builder.Build();
}