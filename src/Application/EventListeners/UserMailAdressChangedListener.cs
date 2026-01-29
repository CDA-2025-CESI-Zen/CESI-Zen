using CesiZen.Domain.Aggregates.Accounts.Events;
using CesiZen.Domain.Aggregates.Core;
using CesiZen.Infrastructure.Services;
using FluentResponse.Interfaces;

namespace CesiZen.Application.EventListeners;
public sealed class UserMailAddressChangedListener(
    IMailService mailService
) : IDomainEventListener<UserMailAddressChanged> {
    
    public Task<IResponse> HandleAsync(UserMailAddressChanged domainEvent, CancellationToken cancellationToken = default) =>
        mailService.TrySendEmailAsync(
            domainEvent.NewMailAddress,
            "Mise à jour de votre compte CESI Zen",
            $"""
            Bonjour,
            L'addresse électronique de votre compte CESI Zen a été changée avec succsès de {domainEvent.OldMailAddress.Address} vers {domainEvent.NewMailAddress.Address}.
            """
        );
}