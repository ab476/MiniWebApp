namespace MiniWebApp.UserApi.DAL.Models;

public class RefreshToken
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string TokenHash { get; set; } = default!;

    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public Guid? ReplacedByTokenId { get; set; }

    public string? CreatedByIp { get; set; }

    public User User { get; set; } = default!;
    public RefreshToken? ReplacedByToken { get; set; }
}
