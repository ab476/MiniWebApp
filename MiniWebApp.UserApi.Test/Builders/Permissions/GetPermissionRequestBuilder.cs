using MiniWebApp.UserApi.Models.Permissions;

namespace MiniWebApp.UserApi.Test.Builders;

/// <summary>
/// Builder for GetPermissionRequest using deferred execution via Lazy fields.
/// </summary>
[BuilderFor(typeof(GetPermissionRequest))]
public partial class GetPermissionRequestBuilder : IBuilder<GetPermissionRequest, GetPermissionRequestBuilder>
{
    public static GetPermissionRequestBuilder Default => New().WithDefaults();

    /// <summary>
    /// Generates a random Guid for the Id.
    /// </summary>
    public GetPermissionRequestBuilder WithRandomId()
    {
        return WithId(Guid.NewGuid());
    }

    /// <summary>
    /// Generates a randomized Permission Code (e.g., "PERM-7A2B9C").
    /// </summary>
    public GetPermissionRequestBuilder WithRandomCode()
    {
        return WithCode($"PERM-{Guid.NewGuid().ToString()[..8].ToUpper()}");
    }

    /// <summary>
    /// Sets sane defaults for a search/get request.
    /// </summary>
    public GetPermissionRequestBuilder WithDefaults()
    {
        return this
            .WithRandomId()
            .WithRandomCode();
    }

    public static GetPermissionRequestBuilder New() => new();

    public static implicit operator GetPermissionRequest(GetPermissionRequestBuilder builder)
    {
        return builder.Build();
    }
}