using CesiZen.Application.Ports;
using CesiZen.Domain.Aggregates.Accounts.Events;
using CesiZen.Domain.Aggregates.Core;
using FluentResponse.Interfaces;

namespace CesiZen.Application.EventListeners;
public sealed class AdminMailAddressChangedListener(
    IMailService mailService
) : IDomainEventListener<AdminMailAddressChanged> {
    
    public Task<IResponse> HandleAsync(AdminMailAddressChanged domainEvent, CancellationToken cancellationToken = default) =>
        mailService.TrySendEmailAsync(
            domainEvent.NewMailAddress,
            "Mise à jour de votre compte administrateur CESI Zen",
            $"""
            Bonjour,
            L'addresse électronique de votre compte administrateur CESI Zen a été changée avec succsès de {domainEvent.OldMailAddress.Address} vers {domainEvent.NewMailAddress.Address}.
            """
        );
}