using CesiZen.Domain.Aggregates.Accounts.ValueObjects;
using CesiZen.Domain.Aggregates.Core;

namespace CesiZen.Domain.Aggregates.Accounts.Events;
public record UserSuspensionChanged(
    Id UserId,
    UserMailAddress UserMailAddress,
    bool Suspension,
    string? Reason
) : DomainEvent;