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
        return WithRandomRoleCode()
            .WithTenantId(Guid.Empty)
            .WithRandomRoleCode()
            .WithCreatedAt(DateTime.UtcNow)
            .WithUserRoles([]);
    }

    #region Domain Helpers

    /// <summary>
    /// Sets the Name and automatically handles the NormalizedName (UPPERCASE).
    /// </summary>
    public RoleBuilder WithRandomRoleCode()
    {
        var roles = new[] { "Manager", "Editor", "Viewer", "Auditor", "Contributor" };
        var name = $"{roles[Random.Shared.Next(roles.Length)]}_{Guid.NewGuid().ToString()[..4]}";

        return WithRoleCode(name);
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