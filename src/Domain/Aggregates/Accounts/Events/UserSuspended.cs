using CesiZen.Domain.Aggregates.Core;

namespace CesiZen.Domain.Aggregates.Accounts.Events;
public record UserSuspensionChanged(
    Id UserId,
    bool Suspension
) : DomainEvent;