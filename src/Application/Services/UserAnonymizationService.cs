using CesiZen.Domain.Aggregates.Accounts;
using CesiZen.Domain.Aggregates.Core;
using CesiZen.Infrastructure.Services;
using FluentResponse;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CesiZen.Application.Services;
public class UserAnonymizationService(
    IServiceProvider                  serviceProvider,
    ILogger<UserAnonymizationService> logger
) : BackgroundService {

    #region PROPERTIES

        private readonly PeriodicTimer timer = new(TimeSpan.FromDays(1));

    #endregion
    #region METHODS
        
        protected override async Task ExecuteAsync(CancellationToken cancellationToken) {

            logger.LogInformation($"Service running.");
            await HandleAsync();
            while (!cancellationToken.IsCancellationRequested && await timer.WaitForNextTickAsync(cancellationToken))
                await HandleAsync();
        }

        private async Task HandleAsync() {

            logger.LogInformation($"Parsing users.");

            using var scope = serviceProvider.CreateScope();
            var repository  = scope.ServiceProvider.GetRequiredService<IRepository<User>>();
            var mailService = scope.ServiceProvider.GetRequiredService<IMailService>();

            var now = DateTime.UtcNow;

            var inactiveUsers = await repository.GetAllAsync(x => MonthsSinceLastActivity(x.LastActivity, now) >= 35 && !x.IsAnonymous);

            foreach (var inactiveUser in inactiveUsers)
                if (inactiveUser.AnonymizationProcessStartedAt is DateTime anonymizationProcessStartedAt && anonymizationProcessStartedAt.AddMonths(1) <= now)
                    await repository
                        .TryUpdateAsync(inactiveUser.Id, x => x.AsAnonymized())
                        .OnSuccessAsync(x => mailService.TrySendEmailAsync(
                            inactiveUser.MailAddress!,
                            "Désactivation de votre compte CESI Zen",
                            $"""
                            Votre compte CESI Zen a été désactivé et anonymisé pour cause d'inactivité.
                            """
                        ));

                else if (inactiveUser.AnonymizationProcessStartedAt is null)
                    await repository
                        .TryUpdateAsync(inactiveUser.Id, x => x.TryStartAnonymizationProcess())
                        .OnSuccessAsync(x => mailService.TrySendEmailAsync(
                            inactiveUser.MailAddress!,
                            "Désactivation de votre compte CESI Zen",
                            $"""
                            Votre compte CESI Zen sera désactivé et anonymisé le {x.AnonymizationProcessStartedAt?.AddMonths(1)} pour cause d'inactivité.
                            Connectez-vous à l'application pour annuler le processus.
                            """
                        ));
    }

        private static int MonthsSinceLastActivity(DateTime lastActivity, DateTime now) =>
            (now.Year - lastActivity.Year) * 12 + now.Month - lastActivity.Month + (now.Day >= lastActivity.Day ? 0 : -1);

    #endregion

}