using CesiZen.Application.Ports;
using CesiZen.Domain.Aggregates.Accounts.Events;
using CesiZen.Domain.Aggregates.Core;
using FluentResponse.Interfaces;

namespace CesiZen.Application.EventListeners;
public sealed class UserAccountCreatedListener(
    IMailService mailService
) : IDomainEventListener<UserAccountCreated> {
    
    public Task<IResponse> HandleAsync(UserAccountCreated domainEvent, CancellationToken cancellationToken = default) =>
        mailService.TrySendEmailAsync(
            domainEvent.AtMailAddress,
            "Votre compte CESI Zen a été créé",
            """
            Bonjour,
            Votre compte CESI Zen a été créé avec succès.
            """
        );
}