namespace CesiZen.Domain.Aggregates.Core;
public interface IDomainEvent {
    public DateTime OccuredAt { get; }
}