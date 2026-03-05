namespace MiniWebApp.Core.Security;

public interface IUserContext
{
    Guid UserId { get; }
    Guid TenantId { get; }
    string Email { get; }
    string UserName { get; }
    bool IsAuthenticated { get; }
    bool IsSuperAdmin { get; }
    IReadOnlySet<string> Roles { get; }
    IReadOnlySet<string> Permissions { get; }
}
