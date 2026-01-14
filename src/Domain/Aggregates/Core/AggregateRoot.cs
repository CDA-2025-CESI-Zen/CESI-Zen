global using Id = ulong;
namespace CesiZen.Domain.Aggregates.Core;
public abstract record AggregateRoot<T>(Id Id = default) where T : AggregateRoot<T> {

    public IEnumerable<IDomainEvent> DomainEvents { get; protected init; } = [];
    public T WithConsumedEvents(out IEnumerable<IDomainEvent> domainEvents) {
        domainEvents = this.DomainEvents;
        return (T)(this with { DomainEvents = [] });
    }
}