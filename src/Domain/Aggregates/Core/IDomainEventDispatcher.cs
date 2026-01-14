namespace CesiZen.Domain.Aggregates.Core;
public interface IDomainEventDispatcher {
    public Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents);
}