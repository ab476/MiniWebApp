namespace MiniWebApp.UserApi.Test.Builders.Tenants;

public partial class DeactivateTenantRequestBuilder : IBuilder<DeactivateTenantRequest, DeactivateTenantRequestBuilder>
{
    private Guid TenantId { get; set; }

    public DeactivateTenantRequestBuilder()
    {
        TenantId = Guid.Empty; // Initialize with an empty GUID, WithDefaults will set a random one.
    }

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

    public DeactivateTenantRequestBuilder WithTenantId(Guid value)
    {
        TenantId = value;
        return this;
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

    public DeactivateTenantRequest Build()
    {
        return new DeactivateTenantRequest(TenantId);
    }

    public static implicit operator DeactivateTenantRequest(DeactivateTenantRequestBuilder builder)
    {
        return builder.Build();
    }

    #endregion
}