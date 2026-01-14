using CesiZen.Domain.Aggregates.Accounts.ValueObjects;
using CesiZen.Domain.Aggregates.Core;

namespace CesiZen.Domain.Aggregates.Accounts.Events;
public record UserMailAddressChanged(
    Id              UserId,
    UserMailAddress OldMailAddress,
    UserMailAddress NewMailAddress
) : DomainEvent;