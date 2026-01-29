using CesiZen.Domain.Aggregates.Core;

namespace CesiZen.Domain.Aggregates.Accounts.Events;
public record UserAnonymizationProcessStarted(
    Id UserId
) : DomainEvent;