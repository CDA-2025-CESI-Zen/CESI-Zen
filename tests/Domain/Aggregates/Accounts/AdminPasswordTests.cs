using CesiZen.Domain.Aggregates.Accounts;
using FluentResponse;

namespace CesiZen.Domain.Tests.Aggregates.Accounts;
public class AdminPasswordTests {

    [Fact]
    public void TryWithPassword_ShouldSucceed() {

        // Arrange
        var admin = Admin
            .TryCreate("test@cesizen.fr", "abcdABCD1234")
            .Unwrap();

        // Act
        var response = admin.TryWithPassword("efghEFGH5678");

        // Assert
        response.Should().BeAssignableTo<ISuccess<Admin>>();
    }

    [Fact]
    public void TryVerifyPassword_WithCorrectPassword_ShouldSucceed() {

        // Arrange
        var admin = Admin
            .TryCreate("test@cesizen.fr", "abcdABCD1234")
            .Unwrap();

        // Act
        var response = admin.TryVerifyPassword("abcdABCD1234");

        // Assert
        response.Should().BeAssignableTo<ISuccess>();
    }

    [Fact]
    public void TryVerifyPassword_WithWrongPassword_ShouldFail() {
        
        // Arrange
        var admin = Admin
            .TryCreate("test@cesizen.fr", "abcdABCD1234")
            .Unwrap();

        // Act
        var response = admin.TryVerifyPassword("WrongPassword");

        // Assert
        response.Should().BeAssignableTo<IFailure>();
    }
}
