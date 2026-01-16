global using Id = ulong;
using FluentResponse.Interfaces;
namespace CesiZen.Domain.Aggregates.Core;
public abstract record AggregateRoot<T>(Id Id = default) where T : AggregateRoot<T> {

    public IEnumerable<IDomainEvent> DomainEvents { get; protected init; } = [];
    public T WithConsumedEvents(out IEnumerable<IDomainEvent> domainEvents) {
        domainEvents = this.DomainEvents;
        return (T)(this with { DomainEvents = [] });
    }

    public virtual Func<IRepository<T>, Task<IResponse<T>>>? RepositoryInvariant { get; }
}