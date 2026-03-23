using CesiZen.Application.Ports;
using CesiZen.Domain.Aggregates.Accounts;
using CesiZen.Domain.Aggregates.Accounts.Events;
using CesiZen.Domain.Aggregates.Core;
using FluentResponse;
using FluentResponse.Interfaces;

namespace CesiZen.Application.EventListeners;
public sealed class UserAnonymizationProcessStartedListener(
    IRepository<User> repository,
    IMailService      mailService
) : IDomainEventListener<UserAnonymizationProcessStarted> {
    
    public Task<IResponse> HandleAsync(UserAnonymizationProcessStarted domainEvent, CancellationToken cancellationToken = default) =>
        repository.TryGetAsync(domainEvent.UserId).OnSuccessAsync(user => mailService.TrySendEmailAsync(
            user.MailAddress!,
            "Désactivation de votre compte CESI Zen",
            $"""
            Votre compte CESI Zen sera désactivé et anonymisé le {domainEvent.OccuredAt.AddMonths(1).Date} pour cause d'inactivité.
            Connectez-vous à l'application pour annuler le processus.
            """
        ));
}