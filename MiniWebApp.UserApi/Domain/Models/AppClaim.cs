namespace MiniWebApp.UserApi.Domain.Models;

public class AppClaim
{
    public required string ClaimCode { get; set; }
    public string? Description { get; set; }
    public required string Category { get; set; }
    public ICollection<RoleClaim> RoleClaims { get; set; } = [];
}
