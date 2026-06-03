using CesiZen.Domain.Aggregates.Accounts.Events;
using CesiZen.Domain.Aggregates.Accounts.ValueObjects;
using CesiZen.Domain.Aggregates.Core;
using FluentResponse;
using FluentResponse.Interfaces;

namespace CesiZen.Domain.Aggregates.Accounts;
public record Admin : AggregateRoot<Admin> {

    #region PROPERTIES

        /// <summary> The mail address of this admin. </summary>
        public AdminMailAddress MailAddress { get; internal init; } = null!;

        /// <summary> The encrypted password of this admin. </summary>
        public Password Password { get; internal init; }

        public override Func<IRepository<Admin>, Task<IResponse<Admin>>>? RepositoryInvariant => async (repository) => 
            this.MailAddress.Address is string mailAddress && await repository.AnyAsync((x) =>
                x.Id != this.Id &&
                x.MailAddress.Address == mailAddress
            ) ? Response.Failure<Admin>(new InvariantException<Admin>($"L'adresse électronique `{mailAddress}` est déjà utilisée par un compte !"))
              : Response.Success(this);

    #endregion
    #region CONSTRUCTORS

        /// <summary>
        /// Returns an admin with an account creation domain event
        /// if the mail address and password are valid.
        /// </summary>
        public static IResponse<Admin> TryCreate(
            string mailAdress,
            string password
        ) => AdminMailAddress
            .TryCreate(mailAdress)
            .OnSuccess(mailAdress => Password
                .TryCreate(password)
                .OnSuccess(password => new Admin {
                    MailAddress  = mailAdress,
                    Password     = password,
                    DomainEvents = [new AdminAccountCreated(mailAdress)]
                })
            );

    #endregion
    #region METHODS

        /// <summary>
        /// Returns a copy of the admin with the given mail address value if valid,
        /// and with a mail address change domain event if it is different from the previous one.
        /// </summary>
        public IResponse<Admin> TryWithMailAddress(string value) =>
            AdminMailAddress
                .TryCreate(value)
                .OnSuccess(mailAddress => (this.MailAddress?.Address != mailAddress.Address)
                    ? this with {
                        DomainEvents = [..this.DomainEvents, new AdminMailAddressChanged(this.Id, this.MailAddress!, mailAddress)],
                        MailAddress  = mailAddress
                    } : this);

        /// <summary> Returns a copy of the admin with the given password hashed value if valid. </summary>
        public IResponse<Admin> TryWithPassword(string value) =>
            Password
                .TryCreate(value)
                .OnSuccess(password => this with { Password = password });

        /// <summary> Tries to verify the admin's password with the given one. </summary>
        public IResponse TryVerifyPassword(string value) =>
            this.Password.TryVerify(value);

    #endregion
    
}