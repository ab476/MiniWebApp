namespace MiniWebApp.UserApi.Domain.Events;

public interface IEvent
{
    DateTime OccurredOn { get; }
}
