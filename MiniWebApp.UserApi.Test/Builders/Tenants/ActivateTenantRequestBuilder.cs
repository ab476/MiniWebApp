using MiniWebApp.UserApi.Models.Tenants;

namespace MiniWebApp.UserApi.Test.Builders.Tenants;


[BuilderFor(typeof(ActivateTenantRequest))]
public partial class ActivateTenantRequestBuilder : IBuilder<ActivateTenantRequest, ActivateTenantRequestBuilder>
{
    /// <summary>
    /// Returns a new instance of the builder with valid defaults.
    /// </summary>
    public static ActivateTenantRequestBuilder Default => new ActivateTenantRequestBuilder().WithDefaults();

    /// <summary>
    /// Sets a fresh Guid by default to ensure the request is valid.
    /// </summary>
    public ActivateTenantRequestBuilder WithDefaults()
    {
        return WithTenantId(Guid.NewGuid());
    }

    #region Domain Helpers

    /// <summary>
    /// Explicitly sets an empty Guid to test "Tenant Not Found" or validation scenarios.
    /// </summary>
    public ActivateTenantRequestBuilder WithEmptyTenantId()
    {
        return WithTenantId(Guid.Empty);
    }

    /// <summary>
    /// Generates a completely new random Guid for the TenantId.
    /// </summary>
    public ActivateTenantRequestBuilder WithNewRandomId()
    {
        return WithTenantId(Guid.NewGuid());
    }

    public static implicit operator ActivateTenantRequest(ActivateTenantRequestBuilder builder)
    {
        return builder.Build();
    }

    #endregion
}