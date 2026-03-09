using MiniWebApp.UserApi.Models.Tenants;

namespace MiniWebApp.UserApi.Test.Builders.Tenants;

/// <summary>
/// Builder for CreateTenantRequest using deferred execution via Lazy fields.
/// </summary>
public partial class CreateTenantRequestBuilder : Builder<CreateTenantRequest, CreateTenantRequestBuilder>
{
    private Lazy<string> _name = new(() => default!);
    private Lazy<string?> _domain = new(() => default);

    public CreateTenantRequestBuilder()
    {
        // Initializing with defaults to avoid immediate evaluation
        _name = new(() => "Default Tenant");
        _domain = new(() => null);
    }

    #region Boilerplate Methods

    public override CreateTenantRequest Build()
    {
        return new CreateTenantRequest
        {
            Name = _name.Value,
            Domain = _domain.Value
        };
    }

    public CreateTenantRequestBuilder WithName(string value)
    {
        _name = new(() => value);
        return this;
    }

    public CreateTenantRequestBuilder WithoutName()
    {
        _name = new(() => default!);
        return this;
    }

    public CreateTenantRequestBuilder WithDomain(string? value)
    {
        _domain = new(() => value);
        return this;
    }

    public CreateTenantRequestBuilder WithoutDomain()
    {
        _domain = new(() => null);
        return this;
    }

    public CreateTenantRequestBuilder WithValuesFrom(CreateTenantRequest example)
    {
        _name = new(() => example.Name);
        _domain = new(() => example.Domain);
        return this;
    }

    #endregion

    #region Custom Extensions (Object Mother Logic)
    /// <summary>
    /// Generates a random Name using a GUID string.
    /// </summary>
    public CreateTenantRequestBuilder WithRandomName()
    {
        _name = new(() => $"Tenant-{RandomString()}");
        return this;
    }

    /// <summary>
    /// Generates a random Domain (e.g., "7a2b9c.com").
    /// </summary>
    public CreateTenantRequestBuilder WithRandomDomain()
    {
        _domain = new(() => $"{RandomString(6)}.com");
        return this;
    }

    /// <summary>
    /// Sets sane defaults for all non-nullable or required fields.
    /// </summary>
    public override CreateTenantRequestBuilder WithDefaults()
    {
        return this
            .WithRandomName()
            .WithRandomDomain();
    }

    #endregion
}
