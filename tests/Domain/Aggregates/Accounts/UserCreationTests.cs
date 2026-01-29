using CesiZen.Domain.Aggregates.Accounts;
using CesiZen.Domain.Aggregates.Accounts.Events;
using CesiZen.Domain.Aggregates.Core;
using CesiZen.Domain.Aggregates.Accounts.ValueObjects;

namespace CesiZen.Domain.Tests.Aggregates.Accounts;
public class UserCreationTests {

    [Fact]
    public void TryCreate_WithValidData_ShouldSucceed() {

        // Act
        var address  = "test@example.com";
        var response = User.TryCreate(address, "abcdABCD1234");

        // Assert
        var assertion = response
            .Should().BeAssignableTo<ISuccess<User>>();
        
        assertion
            .Which.Value.MailAddress?.Address
            .Should().Be(address);

        assertion
            .Which.Value.DomainEvents
            .Should().ContainSingle()
            .Which
            .Should().BeAssignableTo<UserAccountCreated>()
            .Which.AtMailAddress.Address
            .Should().Be(address);
    }

    [Fact]
    public void TryCreate_WithInvalidEmail_ShouldFail() {

        // Act
        var response = User.TryCreate("invalid-email", "abcdABCD1234!");

        // Assert
        response
            .Should().BeAssignableTo<IFailure>()
            .Which.Exception
            .Should().BeAssignableTo<InvariantException<UserMailAddress>>();
    }

    [Fact]
    public void TryCreate_WithUnsafePassword_ShouldFail() {

        // Act
        var response = User.TryCreate("test@example.com", "abcd");

        // Assert
        response
            .Should().BeAssignableTo<IFailure>()
            .Which.Exception
            .Should().BeAssignableTo<InvariantException<Password>>();
    }
}
