using CesiZen.Domain.Aggregates.Accounts;
using CesiZen.Domain.Aggregates.Accounts.Events;
using CesiZen.Domain.Aggregates.Core;
using CesiZen.Domain.Aggregates.Accounts.ValueObjects;

namespace CesiZen.Domain.Tests.Aggregates.Accounts;
public class AdminCreationTests {

    [Fact]
    public void TryCreate_WithValidData_ShouldSucceed() {

        // Act
        var address  = "test@cesizen.fr";
        var response = Admin.TryCreate(address, "abcdABCD1234");

        // Assert
        var assertion = response
            .Should().BeAssignableTo<ISuccess<Admin>>();
        
        assertion
            .Which.Value.MailAddress?.Address
            .Should().Be(address);

        assertion
            .Which.Value.DomainEvents
            .Should().ContainSingle()
            .Which
            .Should().BeAssignableTo<AdminAccountCreated>()
            .Which.AtMailAddress.Address
            .Should().Be(address);
    }

    [Fact]
    public void TryCreate_WithInvalidEmail_ShouldFail() {

        // Act
        var response = Admin.TryCreate("test@example.fr", "abcdABCD1234");

        // Assert
        response
            .Should().BeAssignableTo<IFailure>()
            .Which.Exception
            .Should().BeAssignableTo<InvariantException<AdminMailAddress>>();
    }

    [Fact]
    public void TryCreate_WithUnsafePassword_ShouldFail() {

        // Act
        var response = Admin.TryCreate("test@cesizen.fr", "abcd");

        // Assert
        response
            .Should().BeAssignableTo<IFailure>()
            .Which.Exception
            .Should().BeAssignableTo<InvariantException<Password>>();
    }
}
