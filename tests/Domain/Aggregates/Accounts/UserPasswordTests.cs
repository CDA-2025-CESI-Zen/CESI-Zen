using FluentResponse.Interfaces;
using CesiZen.Domain.Aggregates.Accounts;
using FluentAssertions;
using FluentResponse;

namespace CesiZen.Domain.Tests.Aggregates.Accounts;
public class UserPasswordTests {

    [Fact]
    public void TryWithPassword_ShouldSucceed() {

        // Arrange
        var user = User.TryCreate("test@example.com", "abcdABCD1234").Unwrap();

        // Act
        var response = user.TryWithPassword("efghEFGH5678");

        // Assert
        response.Should().BeAssignableTo<ISuccess<User>>();
    }

    [Fact]
    public void TryWithPassword_OnAnonymousUser_ShouldFail() {

        // Arrange
        var user = User
            .TryCreate("test@example.com", "abcdABCD1234")
            .Unwrap()
            .AsAnonymized();

        // Act
        var response = user.TryWithPassword("efghEFGH5678");

        // Assert
        response.Should().BeAssignableTo<IFailure>();
    }

    [Fact]
    public void TryVerifyPassword_WithCorrectPassword_ShouldSucceed() {

        // Arrange
        var user = User
            .TryCreate("test@example.com", "abcdABCD1234")
            .Unwrap();

        // Act
        var response = user.TryVerifyPassword("abcdABCD1234");

        // Assert
        response.Should().BeAssignableTo<ISuccess>();
    }

    [Fact]
    public void TryVerifyPassword_WithWrongPassword_ShouldFail() {
        
        // Arrange
        var user = User
            .TryCreate("test@example.com", "abcdABCD1234")
            .Unwrap();

        // Act
        var response = user.TryVerifyPassword("WrongPassword");

        // Assert
        response.Should().BeAssignableTo<IFailure>();
    }

    [Fact]
    public void TryVerifyPassword_OnAnonymousUser_ShouldFail() {

        // Arrange
        var user = User
            .TryCreate("test@example.com", "abcdABCD1234")
            .Unwrap()
            .AsAnonymized();

        // Act
        var response = user.TryVerifyPassword("abcdABCD1234");

        // Assert
        response.Should().BeAssignableTo<IFailure>();
    }
}
