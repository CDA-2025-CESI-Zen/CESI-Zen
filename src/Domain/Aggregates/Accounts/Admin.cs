using CesiZen.Domain.Aggregates.Accounts.Events;
using CesiZen.Domain.Aggregates.Accounts.ValueObjects;
using CesiZen.Domain.Aggregates.Core;
using FluentResponse;
using FluentResponse.Interfaces;

namespace CesiZen.Domain.Aggregates.Accounts;
public record Admin : AggregateRoot<Admin> {

    #region PROPERTIES

        public    AdminMailAddress MailAddress { get; protected init; } = null!;
        protected Password         Password    { get; init; }

        public override Func<IRepository<Admin>, Task<IResponse<Admin>>>? RepositoryInvariant => async (repository) => 
            this.MailAddress?.Address is string mailAddress && await repository.AnyAsync((x) =>
                x.Id != this.Id &&
                x.MailAddress != null &&
                x.MailAddress.Address == mailAddress
            ) ? Response.Failure<Admin>(new InvariantException<Admin>($"L'adresse électronique `{mailAddress}` est déjà utilisée par un compte !"))
              : Response.Success(this);

    #endregion
    #region CONSTRUCTORS

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

        public IResponse<Admin> TryWithMailAddress(string value) =>
            AdminMailAddress
                .TryCreate(value)
                .OnSuccess(mailAddress => this with {
                    DomainEvents = [..this.DomainEvents, new AdminMailAddressChanged(this.Id, this.MailAddress, mailAddress)],
                    MailAddress  = mailAddress
                });

        public IResponse<Admin> TryWithPassword(string value) =>
            Password
                .TryCreate(value)
                .OnSuccess(password => this with { Password = password });

        public IResponse TryVerifyPassword(string value) =>
            this.Password.Verify(value) switch {
                true  => Response.Success(),
                false => Response.Failure("Mot de passe incorrect !"),
            };

    #endregion
    
}