using FluentResponse.Interfaces;

namespace CesiZen.Domain.Aggregates.Core;
public interface IDomainEventListener<T> where T : IDomainEvent {
    public Task<IResponse> HandleAsync(T domainEvent, CancellationToken cancellationToken = default);
}