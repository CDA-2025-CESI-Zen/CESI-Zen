using CesiZen.Application.Ports;
using CesiZen.Domain.Aggregates.Accounts.Events;
using CesiZen.Domain.Aggregates.Core;
using FluentResponse;
using FluentResponse.Interfaces;

namespace CesiZen.Application.EventListeners;
public sealed class UserSuspensionChangedListener(
    IMailService mailService
) : IDomainEventListener<UserSuspensionChanged> {
    
    public async Task<IResponse> HandleAsync(UserSuspensionChanged domainEvent, CancellationToken cancellationToken = default) =>
        domainEvent.UserMailAddress is not null
            ? domainEvent.Suspension
                ? await mailService.TrySendEmailAsync(
                    domainEvent.UserMailAddress,
                    "Désactivation de votre compte CESI Zen",
                    "Votre compte CESI Zen a été désactivé" + (
                        domainEvent.Reason is not null
                            ? $" pour la raison suivante : \"{domainEvent.Reason}\"."
                            : "."
                        )

                ) : await mailService.TrySendEmailAsync(
                    domainEvent.UserMailAddress,
                    "Réactivation de votre compte CESI Zen",
                    "Votre compte CESI Zen a été réactivé" + (
                        domainEvent.Reason is not null
                            ? $" pour la raison suivante : \"{domainEvent.Reason}\"."
                            : "."
                        )

                )
            : Response.Success();
}