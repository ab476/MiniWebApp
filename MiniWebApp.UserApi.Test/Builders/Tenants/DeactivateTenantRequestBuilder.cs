using MiniWebApp.UserApi.Models.Tenants;

namespace MiniWebApp.UserApi.Test.Builders.Tenants;

[BuilderFor(typeof(DeactivateTenantRequest))]
public partial class DeactivateTenantRequestBuilder : IBuilder<DeactivateTenantRequest, DeactivateTenantRequestBuilder>
{
    /// <summary>
    /// Returns a new instance of the builder with a valid, random TenantId.
    /// </summary>
    public static DeactivateTenantRequestBuilder Default => new DeactivateTenantRequestBuilder().WithDefaults();

    /// <summary>
    /// Sets a fresh Guid by default to represent a valid deactivation target.
    /// </summary>
    public DeactivateTenantRequestBuilder WithDefaults()
    {
        return WithRandomTenantId();
    }

    #region Domain Helpers

    /// <summary>
    /// Explicitly generates a new random Guid.
    /// Internally calls the generated WithTenantId method.
    /// </summary>
    public DeactivateTenantRequestBuilder WithRandomTenantId()
    {
        return WithTenantId(Guid.NewGuid());
    }

    /// <summary>
    /// Sets the TenantId to Guid.Empty to test edge-case validation scenarios.
    /// </summary>
    public DeactivateTenantRequestBuilder WithEmptyTenantId()
    {
        return WithTenantId(Guid.Empty);
    }

    public static implicit operator DeactivateTenantRequest(DeactivateTenantRequestBuilder builder)
    {
        return builder.Build();
    }

    #endregion
}