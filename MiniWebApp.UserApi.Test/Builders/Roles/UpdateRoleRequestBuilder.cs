namespace MiniWebApp.UserApi.Test.Builders.Roles;

public partial class UpdateRoleRequestBuilder : IBuilder<UpdateRoleRequest, UpdateRoleRequestBuilder>
{
    private string _roleCode = "DEFAULT_CODE";
    private string _displayName = "Default Name";
    private Guid _tenantId = Guid.NewGuid();
    //private string? _description = "Default Description";

    public static UpdateRoleRequestBuilder Default => new UpdateRoleRequestBuilder().WithDefaults();

    public UpdateRoleRequestBuilder WithDefaults()
    {
        return WithRandomName();
            //.WithDescription("Updated role description content."); // This now works
    }

    //// 2. Added the missing method
    //public UpdateRoleRequestBuilder WithDescription(string? description)
    //{
    //    _description = description;
    //    return this;
    //}

    public UpdateRoleRequestBuilder WithDisplayName(string displayName)
    {
        _displayName = displayName;
        return this;
    }

    // Example Build method (assuming your IBuilder interface requires it)
    public UpdateRoleRequest Build()
    {
        //return new UpdateRoleRequest(_roleCode, _name, _description);
        return new UpdateRoleRequest(_roleCode, _displayName, _tenantId);

    }

    #region Domain Helpers

    public UpdateRoleRequestBuilder WithRandomName()
    {
        var adjectives = new[] { "Modified", "Enhanced", "Legacy", "Primary" };
        var titles = new[] { "Access", "Profile", "Member", "Supervisor" };
        var name = $"{adjectives[Random.Shared.Next(adjectives.Length)]} {titles[Random.Shared.Next(titles.Length)]}";

        return WithDisplayName(name);
    }

    public UpdateRoleRequestBuilder WithTenantId(Guid? tenantId = null)
    {
        _tenantId = tenantId ?? Guid.NewGuid();
        return this;
    }
    /// <summary>
    /// Sets a specific RoleCode.
    /// </summary>
    public UpdateRoleRequestBuilder WithRoleCode(string roleCode)
    {
        _roleCode = roleCode;
        return this;
    }

    /// <summary>
    /// Generates a randomized RoleCode (e.g., "ROLE_A1B2C3").
    /// </summary>
    public UpdateRoleRequestBuilder WithRandomRoleCode()
    {
        var randomSuffix = Guid.NewGuid().ToString("N")[..6].ToUpper();
        return WithRoleCode($"ROLE_{randomSuffix}");
    }
    public UpdateRoleRequestBuilder WithInvalidEmptyName()
    {
        return WithDisplayName(string.Empty);
    }

    public static implicit operator UpdateRoleRequest(UpdateRoleRequestBuilder builder)
    {
        return builder.Build();
    }

    #endregion
}