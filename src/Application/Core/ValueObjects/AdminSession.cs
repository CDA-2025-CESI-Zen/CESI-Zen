using CesiZen.Domain.Aggregates.Accounts;

namespace CesiZen.Application.Core.ValueObjects;
public readonly record struct AdminSession(
    string Token,
    Admin  Value
);