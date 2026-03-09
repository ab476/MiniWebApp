using MiniWebApp.UserApi.Models.Tenants;

namespace MiniWebApp.UserApi.Test.Builders.Tenants;

/// <summary>
/// Builder for DeactivateTenantRequest using deferred execution via Lazy fields.
/// </summary>
public partial class DeactivateTenantRequestBuilder : Builder<DeactivateTenantRequest, DeactivateTenantRequestBuilder>
{
    private Lazy<Guid> _tenantId = new(() => Guid.Empty);

    public DeactivateTenantRequestBuilder()
    {
        // Initializing with a default Guid to ensure the Lazy object is ready
        _tenantId = new(() => Guid.NewGuid());
    }

    #region Boilerplate Methods

    /// <summary>
    /// Instantiates the record by evaluating the Lazy TenantId field.
    /// </summary>
    public override DeactivateTenantRequest Build()
    {
        return new DeactivateTenantRequest(
            TenantId: _tenantId.Value
        );
    }

    public DeactivateTenantRequestBuilder WithTenantId(Guid value)
    {
        _tenantId = new(() => value);
        return Instance;
    }

    public DeactivateTenantRequestBuilder WithoutTenantId()
    {
        _tenantId = new(() => default);
        return Instance;
    }

    /// <summary>
    /// Maps the value from an existing request into a new deferred Lazy wrapper.
    /// </summary>
    public DeactivateTenantRequestBuilder WithValuesFrom(DeactivateTenantRequest example)
    {
        _tenantId = new(() => example.TenantId);
        return Instance;
    }

    #endregion

    #region Custom Extensions (Object Mother Logic)

    /// <summary>
    /// Generates a new random Guid for the TenantId.
    /// </summary>
    public DeactivateTenantRequestBuilder WithRandomTenantId()
    {
        _tenantId = new(() => Guid.NewGuid());
        return Instance;
    }

    /// <summary>
    /// Sets sane defaults for the request.
    /// </summary>
    public override DeactivateTenantRequestBuilder WithDefaults()
    {
        return this
            .WithRandomTenantId();
    }

    #endregion
}