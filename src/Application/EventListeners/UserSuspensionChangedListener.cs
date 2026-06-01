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
                    "Suspension de votre compte CESI Zen",
                    "Votre compte CESI Zen a été suspendu" + domainEvent.Reason is not null
                        ? $" pour la raison suivante : \"{domainEvent.Reason}\"."
                        : "."

                ) : await mailService.TrySendEmailAsync(
                    domainEvent.UserMailAddress,
                    "Rétablissement de votre compte CESI Zen",
                    "Votre compte CESI Zen a été rétabli" + domainEvent.Reason is not null
                        ? $" pour la raison suivante : \"{domainEvent.Reason}\"."
                        : "."

                )
            : Response.Success();
}