namespace MiniWebApp.UserApi.Test.Builders.Claims;

public partial class ClaimBuilder : IBuilder<AppClaim, ClaimBuilder>
{
    private string _claimCode = default!;
    private string? _description;
    private string _category = "General";
    private ICollection<RoleClaim> _roleClaims = [];

    public static ClaimBuilder New() => new();

    public static ClaimBuilder Default => New().WithDefaults();

    public ClaimBuilder WithDefaults()
    {
        return WithRandomCode()
            .WithDescription("A system-generated claim for testing.")
            .ForCategory("General")
            .WithRoleClaims([]);
    }

    #region Fluent Setters

    public ClaimBuilder WithCode(string code)
    {
        _claimCode = code.ToLowerInvariant();
        return this;
    }

    public ClaimBuilder WithDescription(string? description)
    {
        _description = description;
        return this;
    }

    public ClaimBuilder ForCategory(string category)
    {
        _category = category;
        return this;
    }

    public ClaimBuilder WithRoleClaims(ICollection<RoleClaim> roleClaims)
    {
        _roleClaims = roleClaims;
        return this;
    }

    #endregion

    #region Domain Helpers

    public ClaimBuilder WithRandomCode()
    {
        var modules = new[] { "users", "tenants", "roles", "reports", "billing" };
        var actions = new[] { "read", "write", "delete", "export", "manage" };

        var randomModule = modules[Random.Shared.Next(modules.Length)];
        var randomAction = actions[Random.Shared.Next(actions.Length)];
        var uniqueTag = Guid.NewGuid().ToString()[..4];

        return WithCode($"{randomModule}.{randomAction}.{uniqueTag}");
    }

    public ClaimBuilder LinkedToRole(string roleCode, Guid tenantId)
    {
        _roleClaims.Add(new RoleClaim
        {
            RoleCode = roleCode,
            TenantId = tenantId,
            ClaimCode = _claimCode
        });

        return this;
    }

    #endregion

    public AppClaim Build() => new()
    {
        ClaimCode = _claimCode,
        Description = _description,
        Category = _category,
        RoleClaims = _roleClaims
    };

    public static implicit operator AppClaim(ClaimBuilder builder) => builder.Build();
}