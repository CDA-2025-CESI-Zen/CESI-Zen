namespace CesiZen.Domain.Aggregates.Core;

/// <summary>
/// An event describing a previous change in a domain entity.
/// </summary>
public interface IDomainEvent {
    public DateTime OccuredAt { get; }
}