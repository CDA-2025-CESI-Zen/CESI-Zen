using CesiZen.Domain.Aggregates.Accounts;
using CesiZen.Domain.Aggregates.Accounts.Events;
using CesiZen.Domain.Aggregates.Core;
using CesiZen.Infrastructure.Services;
using FluentResponse;
using FluentResponse.Interfaces;

namespace CesiZen.Application.EventListeners;
public sealed class UserAnonymizedListener(
    IRepository<User> repository,
    IMailService      mailService
) : IDomainEventListener<UserAnonymized> {
    
    public Task<IResponse> HandleAsync(UserAnonymized domainEvent, CancellationToken cancellationToken = default) =>
        repository.TryGetAsync(domainEvent.UserId).OnSuccessAsync(user => mailService.TrySendEmailAsync(
            user.MailAddress!,
            "Désactivation de votre compte CESI Zen",
            $"""
            Votre compte CESI Zen a été désactivé et anonymisé.
            """
        ));
}