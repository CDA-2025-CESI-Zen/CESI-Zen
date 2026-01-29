namespace CesiZen.Domain.Aggregates.Core;
public abstract record DomainEvent : IDomainEvent {
    public DateTime OccuredAt { get; } = DateTime.UtcNow;
}