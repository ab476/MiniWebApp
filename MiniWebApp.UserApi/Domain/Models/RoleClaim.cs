namespace MiniWebApp.UserApi.Domain.Models;

public class RoleClaim
{
    public Guid TenantId { get; set; }
    public string RoleCode { get; set; } = default!;
    public required string ClaimCode { get; set; }

    public Role Role { get; set; } = default!;
    public AppClaim Claim { get; set; } = default!;
}
