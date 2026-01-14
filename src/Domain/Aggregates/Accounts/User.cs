using CesiZen.Domain.Aggregates.Accounts.Events;
using CesiZen.Domain.Aggregates.Accounts.ValueObjects;
using CesiZen.Domain.Aggregates.Core;
using FluentResponse;
using FluentResponse.Interfaces;

namespace CesiZen.Domain.Aggregates.Accounts;
public record User : AggregateRoot<User> {

    #region PROPERTIES

        public    UserMailAddress? MailAddress { get; protected init; }
        protected Password?        Password    { get; init; }

        public DateTime FirstActivity { get; protected init; } = DateTime.Now;
        public DateTime LastActivity  { get; protected init; } = DateTime.Now;

        public UserDiagnosisResult? FirstDiagnosisResult { get; protected init; }
        public UserDiagnosisResult? LastDiagnosisResult  { get; protected init; }

        public DateTime? AnonymizationProcessStartedAt { get; protected init; }

        public bool IsAnonymous => this.MailAddress is null || this.Password is null;

    #endregion
    #region CONSTRUCTORS

        protected User(UserMailAddress mailAddress, Password password) : base() {
            this.MailAddress = mailAddress;
            this.Password    = password;
        }
        
        public static IResponse<User> TryCreate(
            string mailAdress,
            string password
        ) => UserMailAddress
            .TryCreate(mailAdress)
            .OnSuccess(mailAdress => ValueObjects.Password
                .TryCreate(password)
                .OnSuccess(password => new User(mailAdress, password) { DomainEvents = [new UserAccountCreated(mailAdress)]})
            );

    #endregion
    #region METHODS

        public IResponse<User> TryWithMailAddress(string value) =>
            this.IsAnonymous
                ? Response.Failure<User>(new InvariantException<User>("L'adresse électronique d'un compte anonymisé ne peut être redéfinie !"))
                : UserMailAddress

                    .TryCreate(value)
                    .OnSuccess(mailAddress => this with {
                        DomainEvents = [..this.DomainEvents, new UserMailAddressChanged(this.Id, this.MailAddress!, mailAddress)],
                        MailAddress  = mailAddress
                    });
                

        public IResponse<User> TryWithPassword(string value) =>
            this.IsAnonymous
                ? Response.Failure<User>(new InvariantException<User>("Le mot de passe d'un compte anonymisé ne peut être redéfini !"))
                : ValueObjects.Password
                    .TryCreate(value)
                    .OnSuccess(password => this with { Password = password });

        public User WithNewActivity() =>
            this with { LastActivity = DateTime.Now };

        public User WithNewDiagnosisResult(int score) =>
            this with {
                FirstDiagnosisResult = this.FirstDiagnosisResult ?? new (score),
                LastDiagnosisResult  = new (score),
            };

        public User AsAnonymized() =>
            this with {
                MailAddress                   = null,
                Password                      = null,
                AnonymizationProcessStartedAt = DateTime.Now,
                DomainEvents                  = [..this.DomainEvents, new UserAnonymized(this.Id)]
            };

        public IResponse TryVerifyPassword(string value) =>
            this.Password?.Verify(value) switch {
                true  => Response.Success(),
                false => Response.Failure("Mot de passe incorrect !"),
                null  => Response.Failure("Impossible de vérifier le mot de passe d'un compte anonymisé !")
            };

    #endregion

}