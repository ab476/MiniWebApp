using MiniWebApp.UserApi.Models.Tenants;

namespace MiniWebApp.UserApi.Test.Builders.Tenants;

/// <summary>
/// Builder for UpdateTenantRequest using deferred execution via Lazy fields.
/// </summary>
public partial class UpdateTenantRequestBuilder : Builder<UpdateTenantRequest, UpdateTenantRequestBuilder>
{
    private Lazy<string> _name = new(() => default!);
    private Lazy<string?> _domain = new(() => default);

    public UpdateTenantRequestBuilder()
    {
        // Initializing with sane defaults to avoid immediate evaluation
        _name = new(() => "Updated Tenant Name");
        _domain = new(() => null);
    }

    #region Boilerplate Methods

    /// <summary>
    /// Instantiates the record by evaluating all Lazy backing fields.
    /// </summary>
    public override UpdateTenantRequest Build()
    {
        return new UpdateTenantRequest(
            Name: _name.Value,
            Domain: _domain.Value
        );
    }

    public UpdateTenantRequestBuilder WithName(string value)
    {
        _name = new(() => value);
        return Instance;
    }

    public UpdateTenantRequestBuilder WithoutName()
    {
        _name = new(() => default!);
        return Instance;
    }

    public UpdateTenantRequestBuilder WithDomain(string? value)
    {
        _domain = new(() => value);
        return Instance;
    }

    public UpdateTenantRequestBuilder WithoutDomain()
    {
        _domain = new(() => null);
        return Instance;
    }

    /// <summary>
    /// Shallow clones values from an existing request into deferred Lazy wrappers.
    /// </summary>
    public UpdateTenantRequestBuilder WithValuesFrom(UpdateTenantRequest example)
    {
        _name = new(() => example.Name);
        _domain = new(() => example.Domain);
        return Instance;
    }

    #endregion

    #region Custom Extensions (Object Mother Logic)

    /// <summary>
    /// Generates a randomized Name using the base Builder's RandomString helper.
    /// </summary>
    public UpdateTenantRequestBuilder WithRandomName()
    {
        _name = new(() => $"Updated-{RandomString(10)}");
        return Instance;
    }

    /// <summary>
    /// Generates a random subdomain (e.g., "a1b2c3.net").
    /// </summary>
    public UpdateTenantRequestBuilder WithRandomDomain()
    {
        _domain = new(() => $"{RandomString(6).ToLower()}.net");
        return Instance;
    }

    /// <summary>
    /// Sets all required fields to randomized values for testing.
    /// </summary>
    public override UpdateTenantRequestBuilder WithDefaults()
    {
        return this
            .WithRandomName()
            .WithRandomDomain();
    }

    #endregion
}
