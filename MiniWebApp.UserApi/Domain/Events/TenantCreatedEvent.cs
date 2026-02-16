namespace MiniWebApp.UserApi.Domain.Events;
public record TenantCreatedEvent(
    Guid TenantId,
    string Name,
    string? Domain,
    DateTime OccurredOn
) : IEvent;

public record TenantActivatedEvent(
    Guid TenantId,
    DateTime OccurredOn);

public record TenantDeactivatedEvent(
    Guid TenantId,
    DateTime OccurredOn);