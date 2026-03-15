namespace MiniWebApp.UserApi.Test.Builders.Tenants;


public partial class UpdateTenantRequestBuilder : IBuilder<UpdateTenantRequest, UpdateTenantRequestBuilder>
{
    private string Name { get; set; }
    private string? Domain { get; set; }

    public UpdateTenantRequestBuilder()
    {
        Name = string.Empty;
        Domain = null;
    }
    /// <summary>
    /// Returns a valid update request with randomized data.
    /// </summary>
    public static UpdateTenantRequestBuilder Default => new UpdateTenantRequestBuilder().WithDefaults();

    /// <summary>
    /// Sets defaults that represent a typical, valid update operation.
    /// </summary>
    public UpdateTenantRequestBuilder WithDefaults()
    {
        return WithRandomName()
            .WithRandomDomain();
    }

    #region Domain Helpers

    /// <summary>
    /// Generates a randomized name to simulate an edit.
    /// </summary>
    public UpdateTenantRequestBuilder WithRandomName()
    {
        var prefixes = new[] { "Updated", "New", "Revised", "Modern" };
        var randomName = $"{prefixes[Random.Shared.Next(prefixes.Length)]} {Guid.NewGuid().ToString()[..6]}";
        return WithName(randomName);
    }

    /// <summary>
    /// Generates a randomized domain string.
    /// </summary>
    public UpdateTenantRequestBuilder WithRandomDomain()
    {
        var tlds = new[] { "com", "io", "net", "org" };
        var domain = $"updated-{Guid.NewGuid().ToString()[..4]}.{tlds[Random.Shared.Next(tlds.Length)]}";

        return WithDomain(domain);
    }

    /// <summary>
    /// Sets the domain to null, useful for testing the removal of a custom domain.
    /// </summary>
    public UpdateTenantRequestBuilder WithNullDomain()
    {
        return WithDomain((string?)null);
    }

    /// <summary>
    /// Creates a request that violates "Name Required" validation rules.
    /// </summary>
    public UpdateTenantRequestBuilder WithInvalidEmptyName()
    {
        return WithName(string.Empty);
    }

    public UpdateTenantRequestBuilder WithName(string name)
    {
        Name = name;
        return this;
    }

    public UpdateTenantRequestBuilder WithDomain(string? domain)
    {
        Domain = domain;
        return this;
    }
    public UpdateTenantRequest Build()
    {
        return new UpdateTenantRequest(Name, Domain);
    }

    public static implicit operator UpdateTenantRequest(UpdateTenantRequestBuilder builder)
    {
        return builder.Build();
    }
    #endregion 
}