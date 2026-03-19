using MiniWebApp.UserApi.Infrastructure;

namespace MiniWebApp.UserApi.Domain.Models;

public class TenantHistory : IHistoryEntity
{
    public Guid HistoryId { get; set; } = Guid.NewGuid();
    public Guid EntityId { get; set; }
    public string? Name { get; set; } = default!;
    public string? Domain { get; set; }
    public bool? IsActive { get; set; }

    public AuditAction Action { get; set; }
    public Guid? ChangedBy { get; set; }
    public DateTime ChangedAt { get; set; }
}