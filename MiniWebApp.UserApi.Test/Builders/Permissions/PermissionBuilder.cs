namespace MiniWebApp.UserApi.Test.Builders;

[BuilderFor(typeof(Permission))]
public partial class PermissionBuilder : IBuilder<Permission, PermissionBuilder>
{
    /// <summary>
    /// Returns a new instance of the builder with valid default values.
    /// </summary>
    public static PermissionBuilder Default => new PermissionBuilder().WithDefaults();

    /// <summary>
    /// Sets sane defaults, including a randomized permission code and an empty collection.
    /// </summary>
    public PermissionBuilder WithDefaults()
    {
        return WithId(Guid.NewGuid())
            .WithRandomCode()
            .WithDescription("A system-generated permission for testing.")
            .WithCategory("General")
            .WithRolePermissions([]); // Default to an empty list, not null
    }

    #region Domain Helpers

    /// <summary>
    /// Generates a structured permission code (e.g., "Module.Action.XXXX").
    /// </summary>
    public PermissionBuilder WithRandomCode()
    {
        var modules = new[] { "Users", "Tenants", "Roles", "Reports", "Billing" };
        var actions = new[] { "Read", "Write", "Delete", "Export", "Manage" };

        var randomModule = modules[Random.Shared.Next(modules.Length)];
        var randomAction = actions[Random.Shared.Next(actions.Length)];
        var uniqueTag = Guid.NewGuid().ToString()[..4];

        return WithCode($"{randomModule}.{randomAction}.{uniqueTag}");
    }

    /// <summary>
    /// Helper to assign a specific category.
    /// </summary>
    public PermissionBuilder ForCategory(string category)
    {
        return WithCategory(category);
    }

    /// <summary>
    /// Adds a single RolePermission to the internal collection.
    /// This assumes the generated WithRolePermissions handles the assignment.
    /// </summary>
    public PermissionBuilder LinkedToRole(Guid roleId)
    {
        // In a real scenario, you might have a RolePermissionBuilder.Default.WithRoleId(roleId).Build()
        var rolePermission = new RolePermission { RoleId = roleId };

        
        return WithRolePermissions([rolePermission]);
    }

    public static implicit operator Permission(PermissionBuilder builder)
    {
        return builder.Build();
    }

    #endregion
}