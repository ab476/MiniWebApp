namespace MiniWebApp.UserApi.Domain.Models;

public class TRefreshToken
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string TokenHash { get; set; } = default!;

    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public Guid? ReplacedByTokenId { get; set; }

    public string? CreatedByIp { get; set; }

    public TUser User { get; set; } = default!;
    public TRefreshToken? ReplacedByToken { get; set; }
}
