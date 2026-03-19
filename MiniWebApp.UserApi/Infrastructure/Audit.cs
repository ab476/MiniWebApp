using MiniWebApp.UserApi.Domain;
using System.Threading.Channels;

namespace MiniWebApp.UserApi.Infrastructure;

public enum AuditAction { Create = 0, Update = 1, Delete = 2, Activate = 3, Deactivate = 4 }

public interface IAuditTask
{
    object Data { get; }
    AuditAction Action { get; }
    string EntityName { get; }
    DateTime Timestamp { get; }
    Guid? UserId { get; }
}
public record AuditTask
{
    public static IAuditTask Create<T>(T entity, AuditAction action, Guid? userId, DateTime timestamp) where T : class
    {
        return new AuditTaskData(typeof(T).Name, action, entity, userId, timestamp);
    }
    record AuditTaskData(
        string EntityName,
        AuditAction Action,
        object Data,
        Guid? UserId,
        DateTime Timestamp) : IAuditTask;
}

// A simple interface for your History Records
public interface IHistoryEntity
{
    Guid HistoryId { get; set; }
    Guid EntityId { get; set; }
    AuditAction Action { get; set; }
    Guid? ChangedBy { get; set; }
    DateTime ChangedAt { get; set; }
}

public interface IAuditStore
{
    string EntityName { get; }
    Task SaveAsync(IAuditTask task, UserDbContext db, CancellationToken ct);
}

public class TenantAuditStore : IAuditStore
{
    public string EntityName => nameof(AuditTenantRequest);

    public async Task SaveAsync(IAuditTask task, UserDbContext db, CancellationToken ct)
    {
        // 1. Normalize the input to a collection
        IEnumerable<AuditTenantRequest> requests = task.Data switch
        {
            AuditTenantRequest single => [single],
            IEnumerable<AuditTenantRequest> multiple => multiple,
            _ => []
        };

        if (!requests.Any()) return;

        // 2. Map once using a projection
        var historyList = requests.Select(t => new TenantHistory
        {
            EntityId = t.TenantId,
            Name = t.Name,
            Domain = t.Domain,
            IsActive = t.IsActive,
            Action = task.Action,
            ChangedBy = task.UserId,
            ChangedAt = task.Timestamp
        }).ToArray();

        // 3. Performance: Use AddRange (synchronous memory operation)
        db.TenantHistories.AddRange(historyList);

        // 4. Single DB Roundtrip
        await db.SaveChangesAsync(ct);
    }
}


public interface IAuditChannel
{
    ValueTask PublishAsync(IAuditTask task, CancellationToken ct = default);
    IAsyncEnumerable<IAuditTask> ReadAllAsync(CancellationToken ct = default);
}

public class AuditChannel : IAuditChannel
{
    private readonly Channel<IAuditTask> _channel = Channel.CreateBounded<IAuditTask>(5000);
    public ValueTask PublishAsync(IAuditTask task, CancellationToken ct) => _channel.Writer.WriteAsync(task, ct);
    public IAsyncEnumerable<IAuditTask> ReadAllAsync(CancellationToken ct) => _channel.Reader.ReadAllAsync(ct);
}

public class AuditBackgroundWorker(
    IAuditChannel channel,
    IServiceProvider serviceProvider,
    IEnumerable<IAuditStore> stores) : BackgroundService
{
    private readonly Dictionary<string, IAuditStore> stores = 
        stores.ToDictionary(s => s.EntityName, s => s, StringComparer.InvariantCultureIgnoreCase);
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var task in channel.ReadAllAsync(stoppingToken))
        {
            if (stores.TryGetValue(task.EntityName, out var store))
            {
                using var scope = serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<UserDbContext>();
                await store.SaveAsync(task, db, stoppingToken);
            }
        }
    }
}