namespace MiniWebApp.UserApi.Domain.Models;

public class TAuditLog
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public string EntityName { get; set; } = default!;
    public Guid EntityId { get; set; }

    public string Action { get; set; } = default!;

    public string? OldValues { get; set; }
    public string? NewValues { get; set; }

    public Guid? PerformedBy { get; set; }

    public DateTime PerformedAt { get; set; }

    public string? CorrelationId { get; set; }

    public TTenant Tenant { get; set; } = default!;
}
