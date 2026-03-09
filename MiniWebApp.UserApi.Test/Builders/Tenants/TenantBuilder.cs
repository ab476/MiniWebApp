namespace MiniWebApp.UserApi.Test.Builders.Tenants;

/// <summary>
/// specialized Builder for the Tenant Domain Model using Deferred Execution.
/// </summary>
public partial class TenantBuilder : Builder<Tenant, TenantBuilder>
{
    private Lazy<Guid> _id = new(() => Guid.Empty);
    private Lazy<string> _name = new(() => default!);
    private Lazy<string?> _domain = new(() => default);
    private Lazy<bool> _isActive = new(() => false);
    private Lazy<DateTime> _createdAt = new(() => DateTime.MinValue);
    private Lazy<DateTime?> _updatedAt = new(() => default);

    public TenantBuilder()
    {
        // Initializing with "Safe" defaults
        _id = new(() => Guid.NewGuid());
        _name = new(() => "New Tenant");
        _domain = new(() => null);
        _isActive = new(() => true);
        _createdAt = new(() => DateTime.UtcNow);
        _updatedAt = new(() => null);
    }

    #region Boilerplate Methods

    public override Tenant Build()
    {
        return new Tenant
        {
            Id = _id.Value,
            Name = _name.Value,
            Domain = _domain.Value,
            IsActive = _isActive.Value,
            CreatedAt = _createdAt.Value,
            UpdatedAt = _updatedAt.Value
        };
    }

    public TenantBuilder WithId(Guid value)
    {
        _id = new(() => value);
        return Instance;
    }

    public TenantBuilder WithName(string value)
    {
        _name = new(() => value);
        return Instance;
    }

    public TenantBuilder WithoutName()
    {
        _name = new(() => default!);
        return Instance;
    }

    public TenantBuilder WithDomain(string? value)
    {
        _domain = new(() => value);
        return Instance;
    }

    public TenantBuilder WithoutDomain()
    {
        _domain = new(() => null);
        return Instance;
    }

    public TenantBuilder WithIsActive(bool value)
    {
        _isActive = new(() => value);
        return Instance;
    }

    public TenantBuilder WithCreatedAt(DateTime value)
    {
        _createdAt = new(() => value);
        return Instance;
    }

    public TenantBuilder WithUpdatedAt(DateTime? value)
    {
        _updatedAt = new(() => value);
        return Instance;
    }

    public TenantBuilder WithValuesFrom(Tenant example)
    {
        _id = new(() => example.Id);
        _name = new(() => example.Name);
        _domain = new(() => example.Domain);
        _isActive = new(() => example.IsActive);
        _createdAt = new(() => example.CreatedAt);
        _updatedAt = new(() => example.UpdatedAt);
        return Instance;
    }

    #endregion

    #region Custom Extensions (Object Mother Logic)

    /// <summary>
    /// Sets a random Name and Domain for the Tenant.
    /// </summary>
    public TenantBuilder WithRandomName()
    {
        _name = new(() => $"Tenant-{RandomString(10)}");
        return Instance;
    }

    public TenantBuilder WithRandomDomain()
    {
        _domain = new(() => $"{RandomString(8).ToLower()}.com");
        return Instance;
    }

    /// <summary>
    /// Populates all required fields with valid, randomized data.
    /// </summary>
    public override TenantBuilder WithDefaults()
    {
        return this
            .WithId(Guid.NewGuid())
            .WithRandomName()
            .WithRandomDomain()
            .WithIsActive(true)
            .WithCreatedAt(DateTime.UtcNow);
    }

    #endregion
}
