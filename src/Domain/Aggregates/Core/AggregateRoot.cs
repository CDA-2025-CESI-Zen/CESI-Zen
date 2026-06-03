using FluentResponse.Interfaces;
namespace CesiZen.Domain.Aggregates.Core;
public abstract record AggregateRoot<T>(Id Id = default) where T : AggregateRoot<T> {

    /// <summary>
    /// The entity's unconsumed events.
    /// </summary>
    public IEnumerable<IDomainEvent> DomainEvents { get; protected init; } = [];
    public T WithConsumedEvents(out IEnumerable<IDomainEvent> domainEvents) {
        domainEvents = this.DomainEvents;
        return (T)(this with { DomainEvents = [] });
    }

    /// <summary>
    /// A method that tries to verify the entity's invariants on the repository level.
    /// </summary>
    public virtual Func<IRepository<T>, Task<IResponse<T>>>? RepositoryInvariant { get; }
}