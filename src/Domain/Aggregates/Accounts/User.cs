using CesiZen.Domain.Aggregates.Accounts.Events;
using CesiZen.Domain.Aggregates.Accounts.ValueObjects;
using CesiZen.Domain.Aggregates.Core;
using FluentResponse;
using FluentResponse.Interfaces;

namespace CesiZen.Domain.Aggregates.Accounts;
public record User : AggregateRoot<User> {

    #region PROPERTIES

        /// <summary> The mail address of this user. If none, the user is anonymized. </summary>
        public UserMailAddress? MailAddress { get; internal init; }

        /// <summary> The encrypted password of this user. </summary>
        public Password Password { get; internal init; }



        /// <summary> The date of the first activity of this user. </summary>
        public DateTime FirstActivity { get; internal init; } = DateTime.UtcNow;

        /// <summary> The date of the last activity of this user. </summary>
        public DateTime LastActivity { get; internal init; } = DateTime.UtcNow;



        /// <summary> The first diagnosis' result of this user. </summary>
        public UserDiagnosisResult? FirstDiagnosisResult { get; internal init; }

        /// <summary> The latest diagnosis' result of this user. </summary>
        public UserDiagnosisResult? LastDiagnosisResult { get; internal init; }


        /// <summary> The date at which the user's automatic anonymization process started, if any. </summary>
        public DateTime? AnonymizationProcessStartedAt { get; internal init; }



        /// <summary> Whether or not the user has been anonymized. </summary>
        public bool IsAnonymous => this.MailAddress is null;

        /// <summary> Whether or not the user has been suspended. </summary>
        public bool IsSuspended { get; internal init; }


        /// <summary> The ammount of failed authentication attemps. </summary>
        public int AuthFailedAttemptCount { get; internal init; }



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
        
        /// <summary>
        /// Returns a user with an account creation domain event
        /// if the mail address and password are valid.
        /// </summary>
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

        /// <summary>
        /// Returns a copy of the user with the given mail address value if valid,
        /// and with a mail address change domain event if it is different from the previous one.
        /// </summary>
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
                

        /// <summary> Returns a copy of the user with the given password hashed value if valid. </summary>
        public IResponse<User> TryWithPassword(string value) =>
            this.IsAnonymous
                ? Response.Failure<User>(new InvariantException<User>("Le mot de passe d'un compte anonymisé ne peut être redéfini !"))
                : Password
                    .TryCreate(value)
                    .OnSuccess(password => this with { Password = password });

        /// <summary>
        /// Returns a copy of the user with their latest activity set to now (UTC)
        /// and with their auth failed attempt count reset.
        /// </summary>
        public User WithNewActivity() =>
            this with {
                LastActivity = DateTime.UtcNow,
                AuthFailedAttemptCount = 0
            };

        /// <summary>
        /// Returns a copy of the user with a new diagnosis result
        /// and with their first diagnosis result set to that value if it wasn't already set.
        /// </summary>
        public User WithNewDiagnosisResult(int score) =>
            this with {
                FirstDiagnosisResult = this.FirstDiagnosisResult ?? new (score),
                LastDiagnosisResult  = new (score),
            };

        /// <summary>
        /// Returns a copy of the user with their mail address unset,
        /// their password replaced by noise, and with an anonymization domain event.
        /// </summary>
        public User AsAnonymized() =>
            this with {
                MailAddress  = null,
                Password     = Password.FromNoise(),
                DomainEvents = this.MailAddress is not null
                    ? [..this.DomainEvents, new UserAnonymized(this.Id, this.MailAddress)]
                    : this.DomainEvents
            };

        /// <summary> Returns a copy of the user with a new ongoing anonymization process. </summary>
        public IResponse<User> TryStartAnonymizationProcess() =>
            this.AnonymizationProcessStartedAt is not null
                ? Response.Failure<User>(new InvariantException<User>("Le compte est déjà en train d'être anonymisé !"))
                : Response.Success(this with {
                    AnonymizationProcessStartedAt = DateTime.UtcNow,
                    DomainEvents                  = [..this.DomainEvents, new UserAnonymizationProcessStarted(this.Id)]
                });

        /// <summary>
        /// Returns a copy of the user with their suspension updated
        /// and with a suspension change domain event if the suspension value is different.
        /// </summary>
        public User WithSuspension(bool value, string? reason = null) =>
            this with {
                IsSuspended = value,
                DomainEvents = this.IsSuspended != value && this.MailAddress is not null
                    ? [..this.DomainEvents, new UserSuspensionChanged(this.Id, this.MailAddress, value, reason)]
                    : this.DomainEvents
            };

        /// <summary>
        /// Returns a copy of the user with a new failed authentication attempt.
        /// If it reaches 6, the returned user will be suspended.
        /// </summary>
        public User WithNewFailedAuthAttempt(out bool exceededLimit) {
            var updated = this with {
                AuthFailedAttemptCount = this.AuthFailedAttemptCount + 1
            };

            exceededLimit = updated.AuthFailedAttemptCount is >= 6;
            
            if (exceededLimit)
                updated = updated.WithSuspension(true, "Limite de tentatives de connexion dépassée");

            return updated;
        }

        /// <summary> Tries to verify the user's password with the given one. </summary>
        public IResponse TryVerifyPassword(string value) =>
            !this.IsAnonymous
                ? this.Password.TryVerify(value)
                : Response.Failure("Impossible de vérifier le mot de passe d'un compte anonymisé !");

    #endregion

}