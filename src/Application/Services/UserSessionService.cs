using CesiZen.Application.Core.Exceptions;
using CesiZen.Application.Core.ValueObjects;
using CesiZen.Application.Ports;
using CesiZen.Domain.Aggregates.Accounts;
using CesiZen.Domain.Aggregates.Accounts.ValueObjects;
using CesiZen.Domain.Aggregates.Core;
using FluentResponse;
using FluentResponse.Interfaces;

namespace CesiZen.Application.Services;
public sealed class UserSessionService(
    IRepository<User>                   repository,
    IUserAuthService                    authService,
    IRegistrationValidationCacheService registrationValidationCacheService,
    IPasswordResetCacheService          passwordResetCacheService,
    IMailService                        mailService
) : IUserSessionService {
    
    public Task<IResponse<UserSession>> TryAuthAsync(string mailAddress, string password) =>
        repository
            .TryGetAsync(user => user.MailAddress?.Address == mailAddress)
            .OnSuccessAsync(async user =>
            
                // If the user is suspended, terminate the authentication.
                // Otherwise, try to verify their password.
                user.IsSuspended
                    ? Response.Failure<UserSession>(new Exception("Compte désactivé ! Contactez contact@cesizen.fr pour le rétablir."))
                    : await user
                        .TryVerifyPassword(password)
                        .OnFailureAsync(e => {

                            // If it is invalid, register the failed attempt.
                            var updated = user.WithNewFailedAuthAttempt(out bool exceededLimit);
                            return repository
                                .TryUpdateAsync(user.Id, _ => updated)
                                .OnSuccessAsync(_ => exceededLimit
                                    ? Response.Failure<UserSession>(new Exception("Limite de tentative de connexion dépassée !"))
                                    : Response.Failure<UserSession>(e)
                                );

                        }).OnSuccessAsync(() =>

                            // Otherwise, register the new activity
                            // And generate the session token.
                            repository
                                .TryUpdateAsync(user.Id, user => user.WithNewActivity())
                                .OnSuccessAsync(user =>
                                    authService
                                        .TryGenerateToken(user)
                                        .OnSuccess(token => new UserSession(token, user)))));

    public Task<IResponse<UserSession>> TryRegisterAsync(string mailAddress, string password, Pin pin) =>
        registrationValidationCacheService
            .TryGet(mailAddress)
            .OnFailure(_ => Response.Failure<Pin>(new KeyNotFoundException("Code PIN invalide ou expiré !")))
            .OnSuccessAsync(async cachedPin =>
            
                cachedPin != pin
                    ? Response.Failure<UserSession>(new UnauthorizedAccessException("Code PIN incorrect !"))
                    : await User
                        .TryCreate(mailAddress, password)
                        .OnSuccessAsync(repository.TryAddAsync)
                        .OnSuccessAsync(user => {
                            
                            registrationValidationCacheService.TryDelete(mailAddress);
                            return authService
                                .TryGenerateToken(user)
                                .OnSuccess(token => new UserSession(token, user));
                        })
            );


    public Task<IResponse<UserSession>> TryResetPasswordAsync(string mailAddress, string newPassword, Pin pin) =>
        repository
            .TryGetAsync(x => x.MailAddress?.Address == mailAddress)
            .OnSuccessAsync(user =>
            
                passwordResetCacheService
                    .TryGet(user.Id)
                    .OnFailure(_ => Response.Failure<Pin>(new KeyNotFoundException("Code PIN invalide ou expiré !")))
                    .OnSuccessAsync(async cachedPin => {

                        if (cachedPin != pin)
                            return Response.Failure<UserSession>(new UnauthorizedAccessException("Code PIN incorrect !"));

                        return await repository.TryUpdateAsync(
                            user.Id,
                            user => user.TryWithPassword(newPassword)
                        ).OnSuccessAsync(user => {
                            passwordResetCacheService.TryDelete(user.Id);
                            return authService.TryGenerateToken(user).OnSuccess(token => new UserSession(token, user));
                        });
                    })
            );

    public Task<IResponse<UserSession>> TryUpdateAsync(Id id, string password, Func<User, IResponse<User>> transform) =>
        repository
            .TryGetAsync(id)
            .OnSuccessAsync(user => user
                .TryVerifyPassword(password)
                .OnSuccessAsync(() => repository.TryUpdateAsync(id, transform))
            ).OnSuccessAsync(user => authService
                .TryGenerateToken(user)
                .OnSuccess(token => new UserSession(token, user))
            );

    public Task<IResponse> TryAnonymizeAsync(Id id, string password) =>
        repository
            .TryGetAsync(id)
            .OnSuccessAsync(user => user
                .TryVerifyPassword(password)
                .OnSuccessAsync(() => repository.TryUpdateAsync(id, x => x.AsAnonymized()))
            ).OnSuccessAsync(_ => Response.Success());

    public Task<IResponse> TryDeleteAsync(Id id, string password) =>
        repository
            .TryGetAsync(id)
            .OnSuccessAsync(user => user
                .TryVerifyPassword(password)
                .OnSuccessAsync(() => repository.TryDeleteAsync(id))
            );

    public Task<IResponse> TryRequestRegistrationPINAsync(string mailAddress) =>
        UserMailAddress.TryCreate(mailAddress).OnSuccessAsync(async mailAddress => {

            if (await repository.AnyAsync(x => x.MailAddress?.Address == mailAddress.Address))
                return Response.Failure(new EntityConflictException(typeof(User), nameof(User.MailAddress), mailAddress));

            return await registrationValidationCacheService
                .TryAdd(mailAddress.Address, new Pin())
                .OnFailure(_ => Response.Failure<Pin>(new ArgumentException("Code PIN déjà généré !")))
                .OnSuccessAsync(async pin =>
                
                    await mailService.TrySendEmailAsync(
                        mailAddress,
                        "Demande de création de compte CESI Zen",
                        $"Veuillez entrer ce code PIN pour valider la création de votre compte: {pin}.\nIl expirera dans {registrationValidationCacheService.CacheDuration.Minutes} minutes."
                    )
                );
        });

    public Task<IResponse> TryRequestPasswordResetPINAsync(string mailAddress) =>
        repository
            .TryGetAsync(x => x.MailAddress?.Address == mailAddress)
            .OnSuccessAsync(user =>

                passwordResetCacheService
                    .TryAdd(user.Id, new Pin())
                    .OnFailure(_ => Response.Failure<Pin>(new ArgumentException("Code PIN déjà généré !")))
                    .OnSuccessAsync(pin =>
                    
                        mailService.TrySendEmailAsync(
                            user.MailAddress!,
                            "Demande de changement de mot de passe pour votre compte CESI Zen",
                            $"Veuillez entrer ce code PIN pour changer le mot de passe de votre compte: {pin}.\nIl expirera dans {passwordResetCacheService.CacheDuration.Minutes} minutes.")
                    )
            );

}