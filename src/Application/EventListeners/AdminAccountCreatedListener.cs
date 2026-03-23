using CesiZen.Application.Ports;
using CesiZen.Domain.Aggregates.Accounts.Events;
using CesiZen.Domain.Aggregates.Core;
using FluentResponse.Interfaces;

namespace CesiZen.Application.EventListeners;
public sealed class AdminAccountCreatedListener(
    IMailService mailService
) : IDomainEventListener<AdminAccountCreated> {

    public Task<IResponse> HandleAsync(AdminAccountCreated domainEvent, CancellationToken cancellationToken = default) =>
        mailService.TrySendEmailAsync(
            domainEvent.AtMailAddress,
            "Votre compte administrateur CESI Zen a été créé",
            """
            Bonjour,
            Votre compte administrateur CESI Zen a été créé avec succès.
            """
        );
}