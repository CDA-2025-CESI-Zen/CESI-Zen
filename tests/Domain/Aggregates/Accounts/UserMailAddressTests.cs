using CesiZen.Domain.Aggregates.Accounts;
using CesiZen.Domain.Aggregates.Accounts.Events;
using CesiZen.Domain.Aggregates.Core;
using FluentResponse;

namespace CesiZen.Domain.Tests.Aggregates.Accounts;
public class UserMailAddressTests {
    
    [Fact]
    public void TryWithMailAddress_OnNonAnonymousUser_ShouldSucceed() {

        // Arrange
        var oldAddress = "test@example.com";
        var user       = User
            .TryCreate(oldAddress, "abcdABCD1234")
            .Unwrap();

        // Act
        var newAddress = "new@example.com";
        var response   = user.TryWithMailAddress(newAddress);

        // Assert
        var assertion = response
            .Should().BeAssignableTo<ISuccess<User>>();
        
        assertion
            .Which.Value.MailAddress?.Address
            .Should().Be(newAddress);

        var eventAssertion = assertion
            .Which.Value.DomainEvents.OfType<UserMailAddressChanged>()
            .Should().ContainSingle();

        eventAssertion
            .Which.OldMailAddress
            .Should().Be(oldAddress);

        eventAssertion
            .Which.NewMailAddress
            .Should().Be(newAddress);
    }

    [Fact]
    public void TryWithMailAddress_OnAnonymousUser_ShouldFail() {

        // Arrange
        var user = User
            .TryCreate("test@example.com", "abcdABCD1234")
            .Unwrap()
            .AsAnonymized();

        // Act
        var response = user.TryWithMailAddress("new@example.com");

        // Assert
        response
            .Should().BeAssignableTo<IFailure>()
            .Which.Exception
            .Should().BeAssignableTo<InvariantException<User>>();
    }
}
