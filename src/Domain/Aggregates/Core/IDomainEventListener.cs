using FluentResponse.Interfaces;

namespace CesiZen.Domain.Aggregates.Core;

/// <summary>
/// A service for handling domain event handling.
/// </summary>
public interface IDomainEventListener<T> where T : IDomainEvent {

    /// <summary>
    /// Handles the given domain event.
    /// </summary>
    /// <param name="domainEvent">The domain events to handle.</param>
    public Task<IResponse> HandleAsync(T domainEvent, CancellationToken cancellationToken = default);
    
}