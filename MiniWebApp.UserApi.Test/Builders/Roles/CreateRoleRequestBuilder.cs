namespace MiniWebApp.UserApi.Test.Builders.Roles;

public partial class CreateRoleRequestBuilder : IBuilder<CreateRoleRequest, CreateRoleRequestBuilder>
{
    private string _roleCode = "DEFAULT_CODE";
    private string _displayName = "Default Name";
    //private string? _description = "Initial role description.";
    private Guid _tenantId = Guid.Empty; // Kept for logic, though not in the record yet

    public static CreateRoleRequestBuilder Default => new CreateRoleRequestBuilder().WithDefaults();

    public CreateRoleRequestBuilder WithDefaults()
    {
        return WithRandomRoleCode()
            .WithRandomName();
        //.WithDescription("Initial role description.");
    }

    public CreateRoleRequestBuilder WithRoleCode(string roleCode)
    {
        _roleCode = roleCode;
        return this;
    }

    public CreateRoleRequestBuilder WithDisplayName(string displayName)
    {
        _displayName = displayName;
        return this;
    }

    //public CreateRoleRequestBuilder WithDescription(string? description)
    //{
    //    _description = description;
    //    return this;
    //}

    public CreateRoleRequestBuilder WithTenantId(Guid tenantId)
    {
        _tenantId = tenantId;
        return this;
    }

    public CreateRoleRequest Build()
    {
        return new CreateRoleRequest(_roleCode, _displayName, _tenantId);

    }

    #region Domain Helpers

    public CreateRoleRequestBuilder WithRandomRoleCode()
    {
        return WithRoleCode($"ROLE_{Guid.NewGuid().ToString("N")[..6].ToUpper()}");
    }

    public CreateRoleRequestBuilder WithRandomName()
    {
        var prefixes = new[] { "Global", "Regional", "Department", "Project" };
        var roles = new[] { "Lead", "Coordinator", "Specialist", "Analyst" };
        var name = $"{prefixes[Random.Shared.Next(prefixes.Length)]} {roles[Random.Shared.Next(roles.Length)]}";

        return WithDisplayName(name);
    }

    public CreateRoleRequestBuilder ForTenant(Guid tenantId) => WithTenantId(tenantId);

    public CreateRoleRequestBuilder WithEmptyName() => WithDisplayName(string.Empty);

    public CreateRoleRequestBuilder WithEmptyTenantId() => WithTenantId(Guid.Empty);

    public static implicit operator CreateRoleRequest(CreateRoleRequestBuilder builder) => builder.Build();

    #endregion
}