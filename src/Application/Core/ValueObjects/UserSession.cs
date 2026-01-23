using CesiZen.Domain.Aggregates.Accounts;

namespace CesiZen.Application.Core.ValueObjects;
public readonly record struct UserSession(
    string Token,
    User   User
);