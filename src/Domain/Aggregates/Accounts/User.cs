using CesiZen.Domain.Aggregates.Accounts.Events;
using CesiZen.Domain.Aggregates.Accounts.ValueObjects;
using CesiZen.Domain.Aggregates.Core;
using FluentResponse;
using FluentResponse.Interfaces;

namespace CesiZen.Domain.Aggregates.Accounts;
public record User : AggregateRoot<User> {

    #region PROPERTIES

        public UserMailAddress? MailAddress { get; protected init; }
        public Password         Password    { get; protected init; }

        public DateTime FirstActivity { get; protected init; } = DateTime.UtcNow;
        public DateTime LastActivity  { get; protected init; } = DateTime.UtcNow;

        public UserDiagnosisResult? FirstDiagnosisResult { get; protected init; }
        public UserDiagnosisResult? LastDiagnosisResult  { get; protected init; }

        public DateTime? AnonymizationProcessStartedAt { get; protected init; }

        public bool IsAnonymous => this.MailAddress is null;

        public override Func<IRepository<User>, Task<IResponse<User>>>? RepositoryInvariant => async (repository) => 
            this.MailAddress?.Address is string mailAddress && await repository.AnyAsync((x) =>
                x.Id != this.Id &&
                x.MailAddress?.Address == mailAddress
            ) ? Response.Failure<User>(new InvariantException<User>($"L'adresse électronique `{mailAddress}` est déjà utilisée par un compte !"))
              : Response.Success(this);

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
            .OnSuccess(mailAdress => Password
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
                    .OnSuccess(mailAddress => (this.MailAddress?.Address != mailAddress.Address)
                        ? this with {
                            DomainEvents = [..this.DomainEvents, new UserMailAddressChanged(this.Id, this.MailAddress!, mailAddress)],
                            MailAddress  = mailAddress
                        } : this);
                

        public IResponse<User> TryWithPassword(string value) =>
            this.IsAnonymous
                ? Response.Failure<User>(new InvariantException<User>("Le mot de passe d'un compte anonymisé ne peut être redéfini !"))
                : Password
                    .TryCreate(value)
                    .OnSuccess(password => this with { Password = password });

        public User WithNewActivity() =>
            this with { LastActivity = DateTime.UtcNow };

        public User WithNewDiagnosisResult(int score) =>
            this with {
                FirstDiagnosisResult = this.FirstDiagnosisResult ?? new (score),
                LastDiagnosisResult  = new (score),
            };

        public User AsAnonymized() =>
            this with {
                MailAddress  = null,
                Password     = Password.FromNoise(),
                DomainEvents = [..this.DomainEvents, new UserAnonymized(this.Id)]
            };

        public IResponse<User> TryStartAnonymizationProcess() =>
            this.AnonymizationProcessStartedAt is not null
                ? Response.Failure<User>(new InvariantException<User>("Le compte est déjà en train d'être anonymisé !"))
                : Response.Success(this with {
                    AnonymizationProcessStartedAt = DateTime.UtcNow,
                    DomainEvents                  = [..this.DomainEvents, new UserAnonymizationProcessStarted(this.Id)]
                });

        public IResponse TryVerifyPassword(string value) =>
            !this.IsAnonymous
                ? this.Password.TryVerify(value)
                : Response.Failure("Impossible de vérifier le mot de passe d'un compte anonymisé !");

    #endregion

}