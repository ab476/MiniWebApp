namespace MiniWebApp.UserApi.Test.Builders;

[BuilderFor(typeof(Role))]
public partial class RoleBuilder : IBuilder<Role, RoleBuilder>
{
    /// <summary>
    /// Returns a new instance of the builder.
    /// </summary>
    public static RoleBuilder Default => new RoleBuilder().WithDefaults();

    /// <summary>
    /// Sets sane defaults, including mandatory IDs and normalized strings.
    /// </summary>
    public RoleBuilder WithDefaults()
    {
        return WithId(Guid.NewGuid())
            .WithTenantId(Guid.Empty)
            .WithRandomName()
            .WithDescription("A standard test role.")
            .WithCreatedAt(DateTime.UtcNow)
            .WithRolePermissions([])
            .WithUserRoles([]);
    }

    #region Domain Helpers

    /// <summary>
    /// Sets the Name and automatically handles the NormalizedName (UPPERCASE).
    /// </summary>
    public RoleBuilder WithRandomName()
    {
        var roles = new[] { "Manager", "Editor", "Viewer", "Auditor", "Contributor" };
        var name = $"{roles[Random.Shared.Next(roles.Length)]}_{Guid.NewGuid().ToString()[..4]}";

        return WithName(name)
              .WithNormalizedName(name.ToUpperInvariant());
    }

    /// <summary>
    /// Specifically configures the role as an Administrator.
    /// </summary>
    public RoleBuilder AsAdmin()
    {
        return WithName("Administrator")
              .WithNormalizedName("ADMINISTRATOR")
              .WithDescription("Full system access.");
    }

    /// <summary>
    /// Associates the role with a specific Tenant.
    /// </summary>
    public RoleBuilder ForTenant(Guid tenantId)
    {
        return WithTenantId(tenantId);
    }

    /// <summary>
    /// Allows passing an existing Tenant object, setting both the object and the ID.
    /// </summary>
    public RoleBuilder ForTenant(Tenant tenant)
    {
        return WithTenant(tenant)
              .WithTenantId(tenant.Id);
    }

    public static implicit operator Role(RoleBuilder builder)
    {
        return builder.Build();
    }

    #endregion
}