namespace MiniWebApp.UserApi.Test.Builders.Tenants;

[BuilderFor(typeof(Tenant))]
public partial class TenantBuilder : IBuilder<Tenant, TenantBuilder>
{
    /// <summary>
    /// Returns a new instance of the builder.
    /// </summary>
    public static TenantBuilder Default => new TenantBuilder().WithDefaults();

    /// <summary>
    /// Sets sane, randomized default values to ensure a valid object.
    /// </summary>
    public TenantBuilder WithDefaults()
    {
        return WithId(Guid.NewGuid())
            .WithRandomName()
            .WithRandomDomain()
            .WithIsActive(true)
            .WithCreatedAt(DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 100)));
    }

    #region Domain Helpers

    public TenantBuilder WithRandomName()
    {
        var names = new[] { "Acme Corp", "Globex", "Soylent Corp", "Initech", "Umbrella Corp" };
        var randomName = $"{names[Random.Shared.Next(names.Length)]} {Guid.NewGuid().ToString()[..4]}";

        // This calls the generated partial method
        return WithName(randomName);
    }

    public TenantBuilder WithRandomDomain()
    {
        var domains = new[] { "com", "io", "net", "org", "ai" };
        var randomDomain = $"tenant-{Guid.NewGuid().ToString()[..4]}.{domains[Random.Shared.Next(domains.Length)]}";

        return WithDomain(randomDomain);
    }

    public TenantBuilder Deactivated()
    {
        return WithIsActive(false);
    }

    public static implicit operator Tenant(TenantBuilder builder)
    {
        return builder.Build();
    }

    #endregion
}