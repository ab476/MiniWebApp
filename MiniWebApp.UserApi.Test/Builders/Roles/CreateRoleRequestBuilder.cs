using MiniWebApp.UserApi.Models.Roles;

namespace MiniWebApp.UserApi.Test.Builders;



[BuilderFor(typeof(CreateRoleRequest))]
public partial class CreateRoleRequestBuilder : IBuilder<CreateRoleRequest, CreateRoleRequestBuilder>
{
    /// <summary>
    /// Returns a valid CreateRoleRequest with a random TenantId and Name.
    /// </summary>
    public static CreateRoleRequestBuilder Default => new CreateRoleRequestBuilder().WithDefaults();

    /// <summary>
    /// Sets up the basic requirements for a successful role creation.
    /// </summary>
    public CreateRoleRequestBuilder WithDefaults()
    {
        return WithTenantId(Guid.Empty)
            .WithRandomName()
            .WithDescription("Initial role description.");
    }

    #region Domain Helpers

    /// <summary>
    /// Assigns a random, realistic role name.
    /// </summary>
    public CreateRoleRequestBuilder WithRandomName()
    {
        var prefixes = new[] { "Global", "Regional", "Department", "Project" };
        var roles = new[] { "Lead", "Coordinator", "Specialist", "Analyst" };

        var name = $"{prefixes[Random.Shared.Next(prefixes.Length)]} {roles[Random.Shared.Next(roles.Length)]}";

        return WithName(name);
    }

    /// <summary>
    /// Helper for testing multi-tenancy isolation.
    /// </summary>
    public CreateRoleRequestBuilder ForTenant(Guid tenantId)
    {
        return WithTenantId(tenantId);
    }

    /// <summary>
    /// Sets an empty name to test "Name is Required" validation rules.
    /// </summary>
    public CreateRoleRequestBuilder WithEmptyName()
    {
        return WithName(string.Empty);
    }

    /// <summary>
    /// Sets an empty Guid for the TenantId to test relationship validation.
    /// </summary>
    public CreateRoleRequestBuilder WithEmptyTenantId()
    {
        return WithTenantId(Guid.Empty);
    }

    public static implicit operator CreateRoleRequest(CreateRoleRequestBuilder builder)
    {
        return builder.Build();
    }

    #endregion
}
