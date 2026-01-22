using CesiZen.Application.Core.ValueObjects;
using CesiZen.Domain.Aggregates.Accounts;
using CesiZen.Domain.Aggregates.Accounts.ValueObjects;
using CesiZen.Domain.Aggregates.Core;
using CesiZen.Domain.Aggregates.Diagnoses;
using CesiZen.Infrastructure.Core.Exceptions;
using CesiZen.Infrastructure.Core.ValueObjects;
using CesiZen.Infrastructure.Services;
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
            .OnSuccessAsync(user => user
                .TryVerifyPassword(password)
                .OnSuccess(() => authService.TryGenerateToken(user))
                .OnSuccess(token => new UserSession(token, user))
            );

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