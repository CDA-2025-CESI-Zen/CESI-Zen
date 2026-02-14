using CesiZen.Domain.Aggregates.Accounts;
using CesiZen.Domain.Aggregates.Accounts.Events;
using CesiZen.Domain.Aggregates.Core;
<<<<<<< Updated upstream
using CesiZen.Infrastructure.Services;
using FluentResponse;
=======
>>>>>>> Stashed changes
using FluentResponse.Interfaces;

namespace CesiZen.Application.EventListeners;
public sealed class UserAnonymizedListener(
    IMailService mailService
) : IDomainEventListener<UserAnonymized> {
    
    public Task<IResponse> HandleAsync(UserAnonymized domainEvent, CancellationToken cancellationToken = default) =>
        mailService.TrySendEmailAsync(
            domainEvent.UserMailAddress,
            "Désactivation de votre compte CESI Zen",
            $"""
            Votre compte CESI Zen a été désactivé et anonymisé.
            """
        );
}