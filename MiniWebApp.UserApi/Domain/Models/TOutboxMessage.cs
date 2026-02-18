namespace MiniWebApp.UserApi.Domain.Models;

public class TOutboxMessage
{
    public Guid Id { get; set; }

    public string? AggregateType { get; set; }
    public Guid? AggregateId { get; set; }

    public string Type { get; set; } = default!;

    public string Payload { get; set; } = default!;

    public DateTime OccurredOn { get; set; }

    public DateTime? ProcessedOn { get; set; }

    public string? Error { get; set; }
}
