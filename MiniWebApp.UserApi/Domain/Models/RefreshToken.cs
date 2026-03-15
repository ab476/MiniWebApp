using System.Net;

namespace MiniWebApp.UserApi.Domain.Models;

public class RefreshToken
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public byte[] TokenHash { get; set; } = default!;

    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public Guid? ReplacedByTokenId { get; set; }

    public IPAddress? CreatedByIp { get; set; }

    public User User { get; set; } = default!;
    public RefreshToken? ReplacedByToken { get; set; }
}
