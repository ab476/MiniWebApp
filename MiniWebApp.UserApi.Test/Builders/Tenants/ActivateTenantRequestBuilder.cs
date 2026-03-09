using MiniWebApp.UserApi.Models.Tenants;

namespace MiniWebApp.UserApi.Test.Builders.Tenants;

/// <summary>
/// Builder for ActivateTenantRequest using deferred execution via Lazy fields.
/// </summary>
public partial class ActivateTenantRequestBuilder : Builder<ActivateTenantRequest, ActivateTenantRequestBuilder>
{
    private Lazy<Guid> _tenantId = new(() => Guid.Empty);

    public ActivateTenantRequestBuilder()
    {
        // Initializing with a random Guid as the default
        _tenantId = new(() => Guid.NewGuid());
    }

    #region Boilerplate Methods

    /// <summary>
    /// Instantiates the record by evaluating the Lazy backing field.
    /// </summary>
    public override ActivateTenantRequest Build()
    {
        return new ActivateTenantRequest(
            TenantId: _tenantId.Value
        );
    }

    public ActivateTenantRequestBuilder WithTenantId(Guid value)
    {
        _tenantId = new(() => value);
        return Instance;
    }

    public ActivateTenantRequestBuilder WithoutTenantId()
    {
        _tenantId = new(() => default);
        return Instance;
    }

    /// <summary>
    /// Maps the value from an existing request into a new deferred Lazy wrapper.
    /// </summary>
    public ActivateTenantRequestBuilder WithValuesFrom(ActivateTenantRequest example)
    {
        _tenantId = new(() => example.TenantId);
        return Instance;
    }

    #endregion

    #region Custom Extensions (Object Mother Logic)

    /// <summary>
    /// Generates a new random Guid for the TenantId.
    /// </summary>
    public ActivateTenantRequestBuilder WithRandomTenantId()
    {
        _tenantId = new(() => Guid.NewGuid());
        return Instance;
    }

    /// <summary>
    /// Sets sane defaults for the activation request.
    /// </summary>
    public override ActivateTenantRequestBuilder WithDefaults()
    {
        return this
            .WithRandomTenantId();
    }

    #endregion
}