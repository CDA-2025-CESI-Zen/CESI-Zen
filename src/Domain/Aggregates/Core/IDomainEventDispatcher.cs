using FluentResponse.Interfaces;

namespace CesiZen.Domain.Aggregates.Core;
public interface IDomainEventDispatcher {
    public Task<IResponse> DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
}