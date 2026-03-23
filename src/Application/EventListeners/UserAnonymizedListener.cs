using CesiZen.Application.Ports;
using CesiZen.Domain.Aggregates.Accounts.Events;
using CesiZen.Domain.Aggregates.Core;
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