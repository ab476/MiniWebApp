using MiniWebApp.UserApi.Models.Tenants;

namespace MiniWebApp.UserApi.Test.Builders.Tenants;

[BuilderFor(typeof(CreateTenantRequest))]
public partial class CreateTenantRequestBuilder : IBuilder<CreateTenantRequest, CreateTenantRequestBuilder>
{
    /// <summary>
    /// Entry point for a standard, valid CreateTenantRequest.
    /// </summary>
    public static CreateTenantRequestBuilder Default => new CreateTenantRequestBuilder().WithDefaults();

    /// <summary>
    /// Sets up a valid request to prevent validation failures in happy-path tests.
    /// </summary>
    public CreateTenantRequestBuilder WithDefaults()
    {
        return WithRandomName()
            .WithRandomDomain();
    }

    #region Domain Helpers

    public CreateTenantRequestBuilder WithRandomName()
    {
        var suffixes = new[] { "Solutions", "Systems", "Hub", "Lab", "Digital" };
        var randomName = $"Tenant {Guid.NewGuid().ToString()[..6]} {suffixes[Random.Shared.Next(suffixes.Length)]}";

        // Internally calls the generated WithName(string value)
        return WithName(randomName);
    }

    public CreateTenantRequestBuilder WithRandomDomain()
    {
        var tlds = new[] { "com", "net", "org", "tech" };
        var domain = $"api-{Guid.NewGuid().ToString()[..4]}.{tlds[Random.Shared.Next(tlds.Length)]}";

        return WithDomain(domain);
    }

    /// <summary>
    /// Useful for testing validation logic that requires a non-null but empty name.
    /// </summary>
    public CreateTenantRequestBuilder WithEmptyName()
    {
        return WithName(string.Empty);
    }

    /// <summary>
    /// Sets a name exceeding common database/validation limits (e.g., 256+ chars).
    /// </summary>
    public CreateTenantRequestBuilder WithOverlyLongName()
    {
        return WithName(new string('A', 300));
    }

    public static implicit operator CreateTenantRequest(CreateTenantRequestBuilder builder)
    {
        return builder.Build();
    }

    #endregion
}